import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getGames } from "../services/games";
import { createFavorite, getMyFavorites, deleteFavorite, type Favorite } from "../services/favorites";
import { getGenres, type Genre } from "../services/genres";
import { getStoredCurrentUser } from "../services/auth";
import GameCard from "../components/ui/GameCard";
import type { Difficulty, Game, GameCardGame, GameInsight } from "../types/game";

type SortKey = "popular" | "new";

const PAGE_SIZE = 12;

function getDominantDifficulty(insight: GameInsight | null): Difficulty | null {
  if (!insight) {
    return null;
  }

  const entries: Array<[Difficulty, number]> = [
    ["easy", insight.easy_percentage],
    ["medium", insight.medium_percentage],
    ["hard", insight.hard_percentage],
  ];

  return entries.sort((a, b) => b[1] - a[1])[0][0];
}

function toCardGame(game: Game, insight: GameInsight | null): GameCardGame {
  return {
    id: game.id,
    title: game.name,
    description: game.description ?? "",
    coverImage: game.imageUrl ?? "https://placehold.co/640x400?text=No+image",
    minPlayers: game.minPlayers,
    maxPlayers: game.maxPlayers,
    minPlayTime: game.playTimeMinutes,
    maxPlayTime: game.playTimeMinutes,
    rating: insight?.average_rating ?? game.rating,
    reviewCount: insight?.total_reviews ?? null,
    positivePct: insight?.positive_percentage ?? null,
    difficulty: getDominantDifficulty(insight) ?? (game.learningTimeMinutes <= 15
      ? "easy"
      : game.learningTimeMinutes <= 30 ? "medium" : "hard"),
    tags: insight?.liked_aspects.slice(0, 3).map((aspect) => aspect.aspect) ?? game.tags.slice(0, 3),
    insightsGeneratedAt: insight?.generated_at ?? null,
  };
}

function formatRelativeTime(value: string | null) {
  if (!value) {
    return "No insights yet";
  }

  const generatedAt = new Date(value).getTime();
  const minutes = Math.max(1, Math.round((Date.now() - generatedAt) / 60000));

  if (minutes < 60) {
    return `${minutes} min ago`;
  }

  const hours = Math.round(minutes / 60);
  if (hours < 48) {
    return `${hours} hours ago`;
  }

  return new Date(value).toLocaleDateString();
}

export default function Discover() {
  const [search, setSearch] = useState("");
  const [players, setPlayers] = useState("");
  const [maxTime, setMaxTime] = useState("");
  const [difficulty, setDifficulty] = useState<Difficulty | "">("");
  const [genre, setGenre] = useState("");
  const [sort, setSort] = useState<SortKey>("popular");
  const [user] = useState(() => getStoredCurrentUser());
  const [genres, setGenres] = useState<Genre[]>([]);
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [games, setGames] = useState<GameCardGame[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const navigate = useNavigate();

  const hasFilters = Boolean(search || players || maxTime || difficulty || genre);
  const reviewsShown = useMemo(
    () => games.reduce((sum, game) => sum + (game.reviewCount ?? 0), 0),
    [games],
  );
  const latestInsight = useMemo(
    () => games.map((game) => game.insightsGeneratedAt).filter(Boolean).sort().at(-1) ?? null,
    [games],
  );
  const pageCount = Math.max(1, Math.ceil(games.length / PAGE_SIZE));
  const visibleGames = games.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  useEffect(() => {
    getGenres().then(setGenres).catch((loadError: unknown) => {
      setError(loadError instanceof Error ? loadError.message : "Could not load genres.");
    });
    if (user) {
      getMyFavorites().then(setFavorites).catch((loadError: unknown) => {
        setError(loadError instanceof Error ? loadError.message : "Could not load favorites.");
      });
    }
  }, [user]);

  useEffect(() => {
    let active = true;

    const loadGames = async () => {
      setLoading(true);
      setError(null);

      try {
        const response = await getGames({
          search,
          players,
          maxTime,
          difficulty,
          sort,
        });

        if (!active) {
          return;
        }

        const filtered = genre
          ? response.items.filter((game) => game.tags.some((tag) => tag.toLowerCase() === genre.toLowerCase()))
          : response.items;
        setGames(filtered.map((game) => toCardGame(game, null)));
        setTotal(filtered.length);
        setPage(1);
      } catch (loadError) {
        if (!active) {
          return;
        }

        setError(loadError instanceof Error ? loadError.message : "Could not load games.");
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    void loadGames();

    return () => {
      active = false;
    };
  }, [search, players, maxTime, difficulty, sort, genre]);

  const clearFilters = () => {
    setSearch("");
    setPlayers("");
    setMaxTime("");
    setDifficulty("");
    setGenre("");
    setSort("popular");
  };

  async function toggleFavorite(gameId: number) {
    if (!user) {
      navigate("/auth/login");
      return;
    }
    const current = favorites.find((item) => item.gameId === gameId);
    try {
      if (current) {
        await deleteFavorite(current.id);
        setFavorites((items) => items.filter((item) => item.id !== current.id));
      } else {
        const created = await createFavorite(gameId);
        setFavorites((items) => [...items, created]);
      }
      setError(null);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Could not update favorites.");
    }
  }

  return (
    <div className="discover-page">
      <section className="discover-hero">
        <div className="page-shell">
          <div className="hero-copy">
            <div className="hero-kicker">
              <span aria-hidden="true">✦</span>
              <span>AI-powered review analysis</span>
            </div>
            <h1>
              Discover games through<br />
              <span>player insights</span>
            </h1>
            <p>
              Explore board games using thousands of reviews transformed into clear, useful insights.
            </p>
          </div>

          <div className="stats-bar">
            {[
              { label: "Games in database", value: total.toLocaleString() },
              { label: "Reviews analyzed", value: reviewsShown.toLocaleString() },
              { label: "Updated", value: formatRelativeTime(latestInsight) },
            ].map((stat) => (
              <div key={stat.label}>
                <strong>{stat.value}</strong>
                <span>{stat.label}</span>
              </div>
            ))}
          </div>
        </div>
      </section>

      <div className="filter-bar">
        <div className="page-shell filter-row">
          <div className="search-field">
            <svg aria-hidden="true" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <input
              type="text"
              placeholder="Game name..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </div>

          <FilterSelect label="Players" value={players} onChange={setPlayers} options={[
            { value: '', label: 'Any players' },
            { value: '1', label: 'Solo (1)' },
            { value: '2', label: '2 players' },
            { value: '3', label: '3 players' },
            { value: '4', label: '4 players' },
          ]} />

          <FilterSelect label="Max time" value={maxTime} onChange={setMaxTime} options={[
            { value: '', label: 'Any length' },
            { value: '45', label: 'Under 45 min' },
            { value: '75', label: 'Under 75 min' },
            { value: '120', label: 'Under 2 hours' },
          ]} />

          <FilterSelect label="Difficulty" value={difficulty} onChange={v => setDifficulty(v as Difficulty | '')} options={[
            { value: '', label: 'Any difficulty' },
            { value: 'easy', label: 'Easy' },
            { value: 'medium', label: 'Medium' },
            { value: 'hard', label: 'Hard' },
          ]} />

          <FilterSelect label="Genre" value={genre} onChange={setGenre} options={[
            { value: '', label: 'Any genre' },
            ...genres.map((item) => ({ value: item.name, label: item.name })),
          ]} />

          <FilterSelect label="Sort" value={sort} onChange={v => setSort(v as SortKey)} options={[
            { value: 'popular', label: 'Popular' },
            { value: 'new', label: 'New' },
          ]} />

          {hasFilters && (
            <button type="button" onClick={clearFilters} className="clear-filters">
              Clear filters
            </button>
          )}
        </div>
      </div>

      <main className="page-shell results-section">
        <div className="results-header">
          <p>
            <strong>{total.toLocaleString()}</strong> games found
          </p>
        </div>

        {error && <div className="status-message error-message">{error}</div>}

        {loading ? (
          <div className="status-message">Loading games from the database...</div>
        ) : games.length === 0 ? (
          <div className="empty-state">
            <p>No games match your filters</p>
            <span>Try adjusting your search criteria.</span>
            {hasFilters && (
            <button type="button" onClick={clearFilters}>
              Clear filters
            </button>
            )}
          </div>
        ) : (
          <div className="games-grid">
            {visibleGames.map((game) => (
              <GameCard
                key={game.id}
                game={game}
                isFavorite={favorites.some((item) => item.gameId === game.id)}
                onToggleFavorite={() => void toggleFavorite(game.id)}
              />
            ))}
          </div>
        )}
        {!loading && pageCount > 1 && (
          <nav className="pagination" aria-label="Game pages">
            <button type="button" disabled={page === 1} onClick={() => setPage(page - 1)}>Previous</button>
            <span>Page {page} of {pageCount}</span>
            <button type="button" disabled={page === pageCount} onClick={() => setPage(page + 1)}>Next</button>
          </nav>
        )}
      </main>
    </div>
  );
}

function FilterSelect({
  value,
  onChange,
  options,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  options: { value: string; label: string }[];
}) {
  return (
    <select
      value={value}
      onChange={(event) => onChange(event.target.value)}
      className={value ? "filter-select active" : "filter-select"}
    >
      {options.map((option) => (
        <option key={option.value} value={option.value}>{option.label}</option>
      ))}
    </select>
  );
}