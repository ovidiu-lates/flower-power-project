import { useEffect, useMemo, useState, type FormEvent } from 'react';
import { Link, useOutletContext, useParams } from 'react-router-dom';
import GameCard from '../components/ui/GameCard';
import { ApiError } from '../lib/api';
import { getGames, getGameById } from '../services/games';
import { createFavorite, deleteFavorite, getMyFavorites, type Favorite } from '../services/favorites';
import { createRating, getMyRatings, getRatingsForGame, type Rating } from '../services/ratings';
import { getGenres, type Genre } from '../services/genres';
import { getGameTypes, type GameType } from '../services/gameTypes';
import { upsertPreference, getMyPreference, type Preference } from '../services/preferences';
import { getRecommendations } from '../services/recommendations';
import { changeMyPassword, updateMyProfile } from '../services/users';
import type { Game, GameCardGame } from '../types/game';
import { saveCurrentUser } from '../services/auth';
import type { User } from '../types/user';

type Session = { user: User | null; setUser: (user: User | null) => void };
const useSession = () => useOutletContext<Session>();
const imageFallback = 'https://placehold.co/640x400?text=Flower+Power+Games';

function RequireLogin({ children }: { children: React.ReactNode }) {
  const { user } = useSession();
  if (!user) {
    return (
      <main className="page-shell portal-page">
        <section className="content-panel">
          <p className="eyebrow">Members area</p>
          <h1>Sign in to continue</h1>
          <p>Your account is needed to access this page.</p>
          <Link className="button" to="/auth/login">Log in</Link>
        </section>
      </main>
    );
  }
  return <>{children}</>;
}

function Status({ error, message }: { error?: string | null; message?: string | null }) {
  if (error) return <div className="inline-alert error-message" role="alert">{error}</div>;
  if (message) return <div className="inline-alert success-message" role="status">{message}</div>;
  return null;
}

function toCard(game: Game): GameCardGame {
  return {
    id: game.id,
    title: game.name,
    description: game.description ?? '',
    coverImage: game.imageUrl || imageFallback,
    minPlayers: game.minPlayers,
    maxPlayers: game.maxPlayers,
    minPlayTime: game.playTimeMinutes,
    maxPlayTime: game.playTimeMinutes,
    rating: game.rating,
    reviewCount: null,
    positivePct: null,
    difficulty: null,
    tags: [],
    insightsGeneratedAt: null,
  };
}

function useGames() {
  const [games, setGames] = useState<Game[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => {
    let active = true;
    getGames({})
      .then((response) => { if (active) setGames(response.items); })
      .catch((caught: unknown) => { if (active) setError(caught instanceof Error ? caught.message : 'Could not load games.'); })
      .finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, []);
  return { games, loading, error };
}

function Home() {
  const { games, loading, error } = useGames();
  const featured = useMemo(() => [...games].sort((a, b) => b.rating - a.rating).slice(0, 3), [games]);
  return (
    <main className="home-page">
      <section className="home-hero">
        <div className="page-shell home-hero-inner">
          <div className="hero-copy">
            <span className="hero-kicker">A better way to find your next game</span>
            <h1>Make game night<br /><span>something to look forward to.</span></h1>
            <p>Browse a thoughtful collection of board games, compare the essentials, and find a game that fits your table.</p>
            <div className="hero-actions">
              <Link className="button" to="/discover">Discover games <span aria-hidden="true">→</span></Link>
              <Link className="text-link" to="/about">How it works</Link>
            </div>
          </div>
          <div className="home-orbit" aria-hidden="true"><span>✿</span><small>Find your<br />next favorite</small></div>
        </div>
      </section>
      <section className="page-shell home-featured">
        <header className="section-heading">
          <div><p className="eyebrow">A good place to start</p><h2>Popular around the table</h2></div>
          <Link to="/discover" className="text-link">Browse all games →</Link>
        </header>
        <Status error={error} />
        {loading ? <p className="status-message">Loading featured games…</p> : (
          <div className="games-grid">
            {featured.map((game) => <GameCard key={game.id} game={toCard(game)} />)}
          </div>
        )}
      </section>
      <section className="home-intro">
        <div className="page-shell intro-content">
          <span className="intro-flower" aria-hidden="true">✿</span>
          <div><p className="eyebrow">Less scrolling, more playing</p><h2>Find the right game for your kind of night.</h2>
            <p>Start with the number of players, explore different genres, and save the games you want to try. Your next favorite is out there.</p>
            <Link className="text-link" to="/discover">Explore the collection →</Link>
          </div>
        </div>
      </section>
    </main>
  );
}

function GameDetail() {
  const { id } = useParams();
  const { user } = useSession();
  const [game, setGame] = useState<Omit<Game, 'tags'> | null>(null);
  const [gameGenres, setGameGenres] = useState<string[]>([]);
  const [ratings, setRatings] = useState<Rating[]>([]);
  const [favorite, setFavorite] = useState<Favorite | null>(null);
  const [score, setScore] = useState(5);
  const [review, setReview] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const gameId = Number(id);

  useEffect(() => {
    if (!Number.isInteger(gameId) || gameId < 1) return;
    let active = true;
    Promise.all([
      getGameById(gameId),
      getRatingsForGame(gameId),
      getGenres(),
      user ? getMyFavorites() : Promise.resolve([] as Favorite[]),
    ]).then(([loadedGame, loadedRatings, allGenres, favorites]) => {
      if (!active) return;
      setGame(loadedGame);
      setRatings(loadedRatings);
      setGameGenres(allGenres.filter((genre) => loadedGame.genreIds.includes(genre.id)).map((genre) => genre.name));
      setFavorite(favorites.find((item) => item.gameId === gameId) ?? null);
    }).catch((caught: unknown) => {
      if (active) setError(caught instanceof Error ? caught.message : 'Could not load this game.');
    });
    return () => { active = false; };
  }, [gameId, user]);

  async function toggleFavorite() {
    setError(null);
    try {
      if (favorite) {
        await deleteFavorite(favorite.id);
        setFavorite(null);
      } else {
        setFavorite(await createFavorite(gameId));
      }
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Could not update favorites.');
    }
  }

  async function submitReview(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setMessage(null);
    try {
      const created = await createRating(gameId, score, review.trim());
      setRatings((current) => [created, ...current]);
      setReview('');
      setMessage('Your review has been added.');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Could not submit your review.');
    }
  }

  if (error && !game) return <main className="page-shell portal-page"><Status error={error} /><Link to="/discover" className="text-link">Back to Discover</Link></main>;
  if (!game) return <main className="page-shell portal-page"><p className="status-message">Loading game…</p></main>;
  const average = ratings.length
    ? ratings.reduce((sum, item) => sum + item.score, 0) / ratings.length
    : game.rating;

  return (
    <main className="page-shell portal-page">
      <Link to="/discover" className="back-link">← All games</Link>
      <section className="detail-hero">
        <img src={game.imageUrl || imageFallback} alt="" />
        <div className="detail-copy">
          <p className="eyebrow">Game details</p>
          <h1>{game.name}</h1>
          <p>{game.description || 'A new game to discover. Check the details below and decide if it is right for your table.'}</p>
          <div className="detail-actions">
            {user ? (
              <button className="button" type="button" onClick={() => void toggleFavorite()}>
                {favorite ? 'Remove from favorites' : 'Add to favorites'}
              </button>
            ) : <Link className="button" to="/auth/login">Log in to save</Link>}
            {user?.role.toLowerCase().includes('admin') && (
              <Link className="button button-secondary" to={`/admin/games/${game.id}/edit`}>Edit game</Link>
            )}
          </div>
        </div>
      </section>
      <Status error={error} message={message} />
      <section className="detail-columns">
        <div className="content-panel">
          <p className="eyebrow">At a glance</p>
          <div className="metadata-grid">
            <div><span>Players</span><strong>{game.minPlayers}–{game.maxPlayers}</strong></div>
            <div><span>Play time</span><strong>{game.playTimeMinutes} min</strong></div>
            <div><span>Age</span><strong>{game.minimumAge}+</strong></div>
            <div><span>Rating</span><strong>★ {average ? average.toFixed(1) : '—'} <small>({ratings.length})</small></strong></div>
          </div>
          {gameGenres.length > 0 && <div className="detail-genres">{gameGenres.map((name) => <span className="tag" key={name}>{name}</span>)}</div>}
        </div>
        <section className="content-panel">
          <header className="section-heading compact-heading"><div><p className="eyebrow">Player notes</p><h2>Reviews</h2></div><strong>★ {average ? average.toFixed(1) : '—'}</strong></header>
          {ratings.length ? (
            <div className="review-list">
              {ratings.map((item) => (
                <article className="review-item" key={item.id}>
                  <div><strong>Player {item.userId}</strong><span>★ {item.score}/5 · {new Date(item.createdAt).toLocaleDateString()}</span></div>
                  <p>{item.review || 'No written review.'}</p>
                </article>
              ))}
            </div>
          ) : <p className="muted-copy">No reviews yet. Be the first to share a quick note.</p>}
          {user && (
            <form className="stack-form review-form" onSubmit={(event) => void submitReview(event)}>
              <label>Rating
                <select value={score} onChange={(event) => setScore(Number(event.target.value))}>
                  {[5, 4, 3, 2, 1].map((value) => <option key={value} value={value}>{value} stars</option>)}
                </select>
              </label>
              <label>Short review
                <textarea maxLength={500} rows={3} value={review} onChange={(event) => setReview(event.target.value)} placeholder="What should other players know?" />
              </label>
              <button className="button" type="submit">Share review</button>
            </form>
          )}
        </section>
      </section>
    </main>
  );
}

function Profile() {
  const { username = '' } = useParams();
  const { user } = useSession();
  const isOwner = user?.username.toLowerCase() === decodeURIComponent(username).toLowerCase();
  const { games, error: gamesError } = useGames();
  const [ratings, setRatings] = useState<Rating[]>([]);
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [profileError, setProfileError] = useState<string | null>(null);
  useEffect(() => {
    if (!isOwner) return;
    Promise.all([getMyRatings(), getMyFavorites()])
      .then(([myRatings, myFavorites]) => { setRatings(myRatings); setFavorites(myFavorites); })
      .catch((caught: unknown) => setProfileError(caught instanceof Error ? caught.message : 'Could not load your profile.'));
  }, [isOwner]);
  const favoriteGames = games.filter((game) => favorites.some((favorite) => favorite.gameId === game.id)).slice(0, 4);
  return (
    <main className="page-shell portal-page">
      <section className="profile-header content-panel">
        <div className="avatar-large">{decodeURIComponent(username).slice(0, 2).toUpperCase()}</div>
        <div><p className="eyebrow">Player profile</p><h1>{isOwner ? user?.fullName || username : username}</h1><p className="muted-copy">@{decodeURIComponent(username)}</p></div>
        {isOwner && <Link className="button button-secondary profile-edit" to="/me/edit">Edit profile</Link>}
      </section>
      {isOwner ? (
        <>
          <Status error={profileError ?? gamesError} />
          <section className="content-panel profile-library">
            <header className="section-heading compact-heading"><div><p className="eyebrow">Your collection</p><h2>Favorites</h2></div><Link to="/me/favorites" className="text-link">See all →</Link></header>
            {favoriteGames.length ? <div className="games-grid profile-games">{favoriteGames.map((game) => <GameCard key={game.id} game={toCard(game)} />)}</div> : <p className="muted-copy">Your saved games will show up here.</p>}
          </section>
          <section className="content-panel">
            <p className="eyebrow">Recent notes</p><h2>Recent reviews</h2>
            {ratings.length ? ratings.slice(0, 5).map((rating) => <p className="review-summary" key={rating.id}>★ {rating.score} · {rating.review || 'No written review'}</p>) : <p className="muted-copy">No reviews to show yet.</p>}
          </section>
        </>
      ) : (
        <div className="content-panel"><h2>Profile details are not public yet</h2><p className="muted-copy">The current API does not expose public profile, favorite, or review data for other players.</p></div>
      )}
    </main>
  );
}

function EditProfile() {
  const { user, setUser } = useSession();
  const [displayName, setDisplayName] = useState(user?.fullName ?? '');
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  return (
    <RequireLogin>
      <main className="page-shell portal-page narrow-page">
        <header className="page-heading"><p className="eyebrow">Your account</p><h1>Edit profile</h1><p>Update the name shown on your profile or change your password.</p></header>
        <Status error={error} message={message} />
        <form className="content-panel stack-form" onSubmit={async (event) => {
          event.preventDefault(); setError(null); setMessage(null);
          try {
            if (displayName.trim() !== user?.fullName) {
              const updated = await updateMyProfile(displayName.trim());
              const nextUser = { ...user!, fullName: updated.fullName };
              saveCurrentUser(nextUser); setUser(nextUser);
            }
            if (newPassword || currentPassword || confirmPassword) {
              await changeMyPassword(currentPassword, newPassword, confirmPassword);
              setCurrentPassword(''); setNewPassword(''); setConfirmPassword('');
            }
            setMessage('Your profile has been saved.');
          } catch (caught) {
            setError(caught instanceof Error ? caught.message : 'Could not save profile.');
          }
        }}>
          <label>Display name<input required maxLength={80} value={displayName} onChange={(event) => setDisplayName(event.target.value)} /></label>
          <div className="form-divider"><span>Change password</span></div>
          <label>Current password<input type="password" autoComplete="current-password" value={currentPassword} onChange={(event) => setCurrentPassword(event.target.value)} /></label>
          <label>New password<input type="password" minLength={8} autoComplete="new-password" value={newPassword} onChange={(event) => setNewPassword(event.target.value)} /></label>
          <label>Confirm new password<input type="password" minLength={8} autoComplete="new-password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} /></label>
          <button className="button" type="submit">Save changes</button>
        </form>
      </main>
    </RequireLogin>
  );
}

function Favorites() {
  const { games, loading, error: gamesError } = useGames();
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => { getMyFavorites().then(setFavorites).catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load favorites.')); }, []);
  const savedGames = games.filter((game) => favorites.some((favorite) => favorite.gameId === game.id));
  async function remove(gameId: number) {
    const favorite = favorites.find((item) => item.gameId === gameId);
    if (!favorite) return;
    try { await deleteFavorite(favorite.id); setFavorites((items) => items.filter((item) => item.id !== favorite.id)); }
    catch (caught) { setError(caught instanceof Error ? caught.message : 'Could not remove favorite.'); }
  }
  return (
    <RequireLogin>
      <main className="page-shell portal-page">
        <header className="page-heading"><p className="eyebrow">Your collection</p><h1>My favorites</h1><p>Games you have saved for another game night.</p></header>
        <Status error={error ?? gamesError} />
        {loading ? <p className="status-message">Loading your library…</p> : savedGames.length ? (
          <div className="games-grid">{savedGames.map((game) => <div className="favorite-card" key={game.id}><GameCard game={toCard(game)} /><button type="button" className="favorite-remove" onClick={() => void remove(game.id)}>Remove</button></div>)}</div>
        ) : <div className="empty-state"><p>Your library is waiting</p><span>Save a game from Discover to keep it here.</span><Link className="button" to="/discover">Explore games</Link></div>}
      </main>
    </RequireLogin>
  );
}

const playtimeRanges = {
  short: [0, 30],
  medium: [31, 60],
  long: [61, 240],
} as const;

function Preferences() {
  const { games } = useGames();
  const [genres, setGenres] = useState<Genre[]>([]);
  const [gameTypes, setGameTypes] = useState<GameType[]>([]);
  const [selectedGenres, setSelectedGenres] = useState<number[]>([]);
  const [selectedGameTypes, setSelectedGameTypes] = useState<number[]>([]);
  const [players, setPlayers] = useState('2');
  const [playtime, setPlaytime] = useState<keyof typeof playtimeRanges>('medium');
  const [preference, setPreference] = useState<Preference | null>(null);
  const [preferenceLoading, setPreferenceLoading] = useState(true);
  const [savingPreference, setSavingPreference] = useState(false);
  const [query, setQuery] = useState('');
  const [recommendations, setRecommendations] = useState<Array<{ gameId: number; name: string; reason: string }>>([]);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  useEffect(() => {
    getGenres()
      .then(setGenres)
      .catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load genres.'));
    getGameTypes()
      .then(setGameTypes)
      .catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load game types.'));
    getMyPreference()
      .then((saved) => {
        setPreference(saved);
        if (saved) {
          setSelectedGenres(saved.genreIds);
          setSelectedGameTypes(saved.typeIds);
          setPlayers(String(saved.minPlayers));
          const range = Object.entries(playtimeRanges).find(([, values]) => values[0] === saved.minPlayTime && values[1] === saved.maxPlayTime);
          if (range) setPlaytime(range[0] as keyof typeof playtimeRanges);
        }
      })
      .catch((caught: unknown) => {
        setError(caught instanceof Error ? caught.message : 'Could not load your preferences.');
      })
      .finally(() => setPreferenceLoading(false));
  }, []);
  const selectedPlaytime = playtimeRanges[playtime];
  async function save() {
    setError(null); setMessage(null);
    const next: Preference = {
      minBudget: preference?.minBudget ?? 0,
      maxBudget: preference?.maxBudget ?? 9999,
      minPlayTime: selectedPlaytime[0],
      maxPlayTime: selectedPlaytime[1],
      minPlayers: Number(players),
      maxPlayers: Number(players),
      minimumAge: preference?.minimumAge ?? 1,
      genreIds: selectedGenres,
      typeIds: selectedGameTypes,
    };
    setSavingPreference(true);
    try {
      const saved = await upsertPreference(next);
      setPreference(saved); setMessage('Your preferences have been saved.');
    } catch (caught) {
      setError(caught instanceof ApiError
        ? `Could not save preferences: ${caught.message} (HTTP ${caught.status})`
        : caught instanceof Error ? caught.message : 'Could not save preferences.');
    } finally {
      setSavingPreference(false);
    }
  }
  async function askRecommendations(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError(null); setMessage(null);
    try {
      const result = await getRecommendations(query.trim());
      setRecommendations(result.recommendations ?? []);
      setMessage('Recommendations are ready.');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Could not get recommendations.');
    }
  }
  const gamesById = new Map(games.map((game) => [game.id, game]));
  return (
    <RequireLogin>
      <main className="page-shell portal-page">
        <header className="page-heading"><p className="eyebrow">Make it yours</p><h1>Preferences &amp; AI</h1><p>Choose what suits your table and ask for a tailored suggestion.</p></header>
        <Status error={error} message={message} />
        <div className="preferences-ai-layout">
          <section className="content-panel stack-form preference-panel">
            <div><p className="eyebrow">Tune your table</p><h2>Game preferences</h2><p className="muted-copy">Recommendations can use these settings as a starting point.</p></div>
            {preferenceLoading && <p className="muted-copy" role="status">Loading saved preferences…</p>}
            {!preferenceLoading && !preference && <p className="preference-empty-note">No saved preferences yet. Choose your preferences and save to create them.</p>}
            <fieldset className="genre-options"><legend>Favorite genres</legend>
              {genres.map((genre) => <label key={genre.id}><input type="checkbox" checked={selectedGenres.includes(genre.id)} onChange={() => setSelectedGenres((items) => items.includes(genre.id) ? items.filter((id) => id !== genre.id) : [...items, genre.id])} />{genre.name}</label>)}
            </fieldset>
            <fieldset className="genre-options"><legend>Favorite game types</legend>
              {gameTypes.map((gameType) => <label key={gameType.id}><input type="checkbox" checked={selectedGameTypes.includes(gameType.id)} onChange={() => setSelectedGameTypes((items) => items.includes(gameType.id) ? items.filter((id) => id !== gameType.id) : [...items, gameType.id])} />{gameType.name}</label>)}
            </fieldset>
            <label>Players<select value={players} onChange={(event) => setPlayers(event.target.value)}>{[1, 2, 3, 4, 5, 6, 8].map((number) => <option key={number} value={number}>{number} players</option>)}</select></label>
            <label>Play time<select value={playtime} onChange={(event) => setPlaytime(event.target.value as keyof typeof playtimeRanges)}><option value="short">Short (up to 30 min)</option><option value="medium">Medium (31–60 min)</option><option value="long">Long (61+ min)</option></select></label>
            <button className="button" type="button" disabled={preferenceLoading || savingPreference} onClick={() => void save()}>{savingPreference ? 'Saving…' : 'Save preferences'}</button>
          </section>
          <section className="content-panel stack-form ai-box recommendation-panel">
            <div><p className="eyebrow">A little help from AI</p><h2>What should we play?</h2><p className="muted-copy">Tell us what your group is in the mood for. Your recommendations will appear here.</p></div>
            <form onSubmit={(event) => void askRecommendations(event)}>
              <label className="visually-hidden" htmlFor="recommendation-prompt">Describe your game night</label>
              <textarea id="recommendation-prompt" required maxLength={1000} rows={4} value={query} onChange={(event) => setQuery(event.target.value)} placeholder="We have four players and want something quick and cooperative…" />
              <button className="button" type="submit">Get recommendations</button>
            </form>
            {recommendations.length > 0 ? (
              <div className="recommendation-results">
                <div className="recommendation-results-heading"><h3>Your suggestions</h3><span>{recommendations.length} games</span></div>
                {recommendations.map((item) => {
                  const game = gamesById.get(item.gameId);
                  return <Link className="recommendation-item" to={`/games/${item.gameId}`} key={item.gameId}>
                    <span className="recommendation-number">✦</span>
                    <span className="recommendation-copy"><strong>{item.name || game?.name}</strong><span>{item.reason}</span></span>
                    <span className="recommendation-arrow" aria-hidden="true">→</span>
                  </Link>;
                })}
              </div>
            ) : (
              <div className="recommendation-empty"><span aria-hidden="true">✿</span><strong>Room for a good idea</strong><p>Share the kind of night you have in mind and we’ll help you find a match.</p></div>
            )}
          </section>
        </div>
      </main>
    </RequireLogin>
  );
}

function StaticPage({ page }: { page: 'About' | 'Terms' | 'Privacy' }) {
  const content = {
    About: 'Flower Power Games helps you explore board games and find a good fit for your next game night.',
    Terms: 'Use Flower Power Games respectfully. Game information and community reviews are provided to help players discover options.',
    Privacy: 'Your account details are used to provide sign-in, saved games, and personalized features. You can update your profile or sign out at any time.',
  };
  return <main className="page-shell portal-page narrow-page"><section className="content-panel"><p className="eyebrow">Flower Power Games</p><h1>{page}</h1><p>{content[page]}</p><Link className="text-link" to="/discover">Discover games →</Link></section></main>;
}

function NotFound() {
  return <main className="page-shell portal-page"><section className="content-panel not-found"><p className="eyebrow">404 · Not found</p><h1>This page took a wrong turn.</h1><p className="muted-copy">The page may have moved, or the address may be incorrect.</p><Link className="button" to="/">Back home</Link></section></main>;
}

const PortalPages = { Home, GameDetail, Profile, EditProfile, Favorites, Preferences, StaticPage, NotFound };
export default PortalPages;
