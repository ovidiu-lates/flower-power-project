import { useEffect, useState, type FormEvent } from 'react';
import { Link, useNavigate, useOutletContext, useParams } from 'react-router-dom';
import { deleteUser, getUsers, updateUser, type ApiUser } from '../services/users';
import { getRoles, type Role } from '../services/roles';
import { getGenres, type Genre } from '../services/genres';
import { createGenre, deleteGenre, updateGenre } from '../services/genres';
import { createGameType, deleteGameType, getGameTypes, updateGameType, type GameType } from '../services/gameTypes';
import { createGame, deleteGame, getGameById, getGames, updateGame } from '../services/games';
import { getAiUsageForUser, updateAiUsageLimit, type AiUsage } from '../services/aiUsage';
import type { Game } from '../types/game';
import type { User } from '../types/user';

type Session = { user: User | null };
const useSession = () => useOutletContext<Session>();
const emptyGame: Omit<Game, 'id' | 'rating'> = {
  name: '',
  description: '',
  price: 0,
  minPlayers: 1,
  maxPlayers: 4,
  playTimeMinutes: 30,
  learningTimeMinutes: 15,
  minimumAge: 8,
  imageUrl: '',
  genreIds: [],
  typeIds: [],
  tags: [],
};

function AdminGate({ children }: { children: React.ReactNode }) {
  const { user } = useSession();
  if (!user) return <main className="page-shell portal-page"><section className="content-panel"><h1>Admin access required</h1><p>Sign in with an administrator account to continue.</p><Link className="button" to="/auth/login">Log in</Link></section></main>;
  if (!user.role.toLowerCase().includes('admin')) return <main className="page-shell portal-page"><section className="content-panel"><h1>Access unavailable</h1><p>This area is for administrators.</p><Link className="text-link" to="/">Return home →</Link></section></main>;
  return <>{children}</>;
}

function Dashboard() {
  const [counts, setCounts] = useState<{ games: number; users: number } | null>(null);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => {
    Promise.all([getGames({}), getUsers()])
      .then(([games, users]) => setCounts({ games: games.total, users: users.length }))
      .catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load dashboard.'));
  }, []);
  return (
    <AdminGate>
      <main className="page-shell portal-page">
        <header className="page-heading"><p className="eyebrow">Admin</p><h1>Dashboard</h1><p>A quick overview and shortcuts to management.</p></header>
        {error && <div className="inline-alert error-message" role="alert">{error}</div>}
        <div className="admin-counts">
          <div className="content-panel"><span>Games</span><strong>{counts?.games ?? '—'}</strong><Link to="/admin/games">Manage games →</Link></div>
          <div className="content-panel"><span>Users</span><strong>{counts?.users ?? '—'}</strong><Link to="/admin/users">Manage users &amp; AI usage →</Link></div>
        </div>
      </main>
    </AdminGate>
  );
}

function Games() {
  const [games, setGames] = useState<Game[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  function load() {
    setLoading(true);
    getGames({}).then((result) => setGames(result.items))
      .catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load games.'))
      .finally(() => setLoading(false));
  }
  useEffect(() => {
    let active = true;
    getGames({}).then((result) => { if (active) setGames(result.items); })
      .catch((caught: unknown) => { if (active) setError(caught instanceof Error ? caught.message : 'Could not load games.'); })
      .finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, []);
  async function remove(id: number, name: string) {
    if (!window.confirm(`Delete "${name}"? This action cannot be undone.`)) return;
    try { await deleteGame(id); load(); }
    catch (caught) { setError(caught instanceof Error ? caught.message : 'Could not delete game.'); }
  }
  return (
    <AdminGate>
      <main className="page-shell portal-page">
        <header className="section-heading"><div className="page-heading"><p className="eyebrow">Admin</p><h1>Games</h1><p>Keep the game catalog up to date.</p></div><Link className="button" to="/admin/games/new">Add a game</Link></header>
        {error && <div className="inline-alert error-message" role="alert">{error}</div>}
        {loading ? <p className="status-message">Loading games…</p> : (
          <div className="table-wrap"><table><thead><tr><th>Game</th><th>Players</th><th>Play time</th><th>Actions</th></tr></thead><tbody>
            {games.map((game) => <tr key={game.id}><td><strong>{game.name}</strong></td><td>{game.minPlayers}–{game.maxPlayers}</td><td>{game.playTimeMinutes} min</td><td className="table-actions"><Link to={`/admin/games/${game.id}/edit`}>Edit</Link><button type="button" className="danger-link" onClick={() => void remove(game.id, game.name)}>Delete</button></td></tr>)}
          </tbody></table></div>
        )}
      </main>
    </AdminGate>
  );
}

function GameEditor() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [game, setGame] = useState(emptyGame);
  const [genres, setGenres] = useState<Genre[]>([]);
  const [gameTypes, setGameTypes] = useState<GameType[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  useEffect(() => {
    getGenres().then(setGenres).catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load genres.'));
    getGameTypes().then(setGameTypes).catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load game types.'));
    if (id) getGameById(Number(id)).then((existing) => {
      setGame({ ...existing, tags: [] });
    }).catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load game.'));
  }, [id]);
  function update<K extends keyof typeof game>(key: K, value: (typeof game)[K]) {
    setGame((current) => ({ ...current, [key]: value }));
  }
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError(null); setSaving(true);
    try {
      if (id) await updateGame(Number(id), game);
      else await createGame(game);
      navigate('/admin/games');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Could not save game.');
    } finally { setSaving(false); }
  }
  return (
    <AdminGate>
      <main className="page-shell portal-page narrow-page">
        <header className="page-heading"><p className="eyebrow">Admin · Games</p><h1>{id ? 'Edit game' : 'Add a game'}</h1></header>
        {error && <div className="inline-alert error-message" role="alert">{error}</div>}
        <form className="content-panel stack-form" onSubmit={(event) => void submit(event)}>
          <label>Title<input required maxLength={120} value={game.name} onChange={(event) => update('name', event.target.value)} /></label>
          <label>Description<textarea rows={5} value={game.description ?? ''} onChange={(event) => update('description', event.target.value)} /></label>
          <fieldset className="genre-options"><legend>Genres</legend>{genres.map((genre) => <label key={genre.id}><input type="checkbox" checked={game.genreIds.includes(genre.id)} onChange={() => update('genreIds', game.genreIds.includes(genre.id) ? game.genreIds.filter((value) => value !== genre.id) : [...game.genreIds, genre.id])} />{genre.name}</label>)}</fieldset>
          <fieldset className="genre-options"><legend>Game types</legend>{gameTypes.map((gameType) => <label key={gameType.id}><input type="checkbox" checked={game.typeIds.includes(gameType.id)} onChange={() => update('typeIds', game.typeIds.includes(gameType.id) ? game.typeIds.filter((value) => value !== gameType.id) : [...game.typeIds, gameType.id])} />{gameType.name}</label>)}</fieldset>
          <div className="field-pair">
            <label>Minimum players<input required type="number" min={1} value={game.minPlayers} onChange={(event) => update('minPlayers', Number(event.target.value))} /></label>
            <label>Maximum players<input required type="number" min={1} value={game.maxPlayers} onChange={(event) => update('maxPlayers', Number(event.target.value))} /></label>
          </div>
          <label>Play time (minutes)<input required type="number" min={1} value={game.playTimeMinutes} onChange={(event) => update('playTimeMinutes', Number(event.target.value))} /></label>
          <label>Cover image URL<input type="url" value={game.imageUrl ?? ''} onChange={(event) => update('imageUrl', event.target.value)} placeholder="https://…" /></label>
          <div className="form-actions"><Link className="button button-secondary" to="/admin/games">Cancel</Link><button className="button" disabled={saving}>{saving ? 'Saving…' : 'Save game'}</button></div>
        </form>
      </main>
    </AdminGate>
  );
}

type LookupItem = { id: number; name: string };
type LookupManagementProps<T extends LookupItem> = {
  title: string;
  description: string;
  loadItems: () => Promise<T[]>;
  createItem: (name: string) => Promise<T>;
  updateItem: (id: number, name: string) => Promise<T>;
  deleteItem: (id: number) => Promise<void>;
};

function LookupManagement<T extends LookupItem>({
  title,
  description,
  loadItems,
  createItem,
  updateItem,
  deleteItem,
}: LookupManagementProps<T>) {
  const [items, setItems] = useState<T[]>([]);
  const [newName, setNewName] = useState('');
  const [editing, setEditing] = useState<Record<number, string>>({});
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let active = true;
    loadItems()
      .then((result) => { if (active) setItems(result); })
      .catch((caught: unknown) => { if (active) setError(caught instanceof Error ? caught.message : `Could not load ${title.toLowerCase()}.`); })
      .finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, [loadItems, title]);

  async function add(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const name = newName.trim();
    if (!name) return;
    setError(null);
    try {
      const created = await createItem(name);
      setItems((current) => [...current, created].sort((left, right) => left.name.localeCompare(right.name)));
      setNewName('');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : `Could not create ${title.toLowerCase().replace(/s$/, '')}.`);
    }
  }

  async function save(id: number) {
    const name = editing[id]?.trim();
    if (!name) return;
    setError(null);
    try {
      const updated = await updateItem(id, name);
      setItems((current) => current.map((item) => item.id === id ? updated : item));
      setEditing((current) => {
        const next = { ...current };
        delete next[id];
        return next;
      });
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : `Could not update ${title.toLowerCase().replace(/s$/, '')}.`);
    }
  }

  async function remove(item: T) {
    if (!window.confirm(`Delete "${item.name}"? Games assigned to it may be affected.`)) return;
    setError(null);
    try {
      await deleteItem(item.id);
      setItems((current) => current.filter((entry) => entry.id !== item.id));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : `Could not delete ${title.toLowerCase().replace(/s$/, '')}.`);
    }
  }

  return (
    <AdminGate>
      <main className="page-shell portal-page">
        <header className="page-heading"><p className="eyebrow">Admin · Game catalog</p><h1>{title}</h1><p>{description}</p></header>
        {error && <div className="inline-alert error-message" role="alert">{error}</div>}
        <form className="content-panel lookup-create-form" onSubmit={(event) => void add(event)}>
          <label htmlFor="lookup-name">Add {title.toLowerCase().replace(/s$/, '')}</label>
          <div className="lookup-create-controls">
            <input id="lookup-name" required maxLength={80} value={newName} onChange={(event) => setNewName(event.target.value)} placeholder={`Enter a ${title.toLowerCase().replace(/s$/, '')} name`} />
            <button className="button" type="submit">Add</button>
          </div>
        </form>
        <div className="table-wrap lookup-table">
          <table>
            <thead><tr><th>{title.slice(0, -1)}</th><th>Actions</th></tr></thead>
            <tbody>
              {loading ? <tr><td colSpan={2}>Loading {title.toLowerCase()}…</td></tr> : items.length === 0 ? <tr><td colSpan={2}>No {title.toLowerCase()} found.</td></tr> : items.map((item) => (
                <tr key={item.id}>
                  <td>{editing[item.id] === undefined ? <strong>{item.name}</strong> : <input aria-label={`Edit ${item.name}`} maxLength={80} value={editing[item.id]} onChange={(event) => setEditing((current) => ({ ...current, [item.id]: event.target.value }))} />}</td>
                  <td className="table-actions">
                    {editing[item.id] === undefined ? (
                      <button type="button" onClick={() => setEditing((current) => ({ ...current, [item.id]: item.name }))}>Edit</button>
                    ) : (
                      <>
                        <button type="button" onClick={() => void save(item.id)}>Save</button>
                        <button type="button" onClick={() => setEditing((current) => {
                          const next = { ...current };
                          delete next[item.id];
                          return next;
                        })}>Cancel</button>
                      </>
                    )}
                    <button type="button" className="danger-link" onClick={() => void remove(item)}>Delete</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </main>
    </AdminGate>
  );
}

const Genres = () => (
  <LookupManagement
    title="Genres"
    description="Create, rename, and remove the genres used to categorize games."
    loadItems={getGenres}
    createItem={createGenre}
    updateItem={updateGenre}
    deleteItem={deleteGenre}
  />
);

const GameTypes = () => (
  <LookupManagement<GameType>
    title="Game types"
    description="Manage the game types available for catalog entries."
    loadItems={getGameTypes}
    createItem={createGameType}
    updateItem={updateGameType}
    deleteItem={deleteGameType}
  />
);

function Users() {
  const [users, setUsers] = useState<ApiUser[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [aiUsage, setAiUsage] = useState<Record<number, AiUsage | null>>({});
  const [promptLimits, setPromptLimits] = useState<Record<number, string>>({});
  const [error, setError] = useState<string | null>(null);
  function load() {
    Promise.all([getUsers(), getRoles()])
      .then(async ([allUsers, allRoles]) => {
        setUsers(allUsers);
        setRoles(allRoles);
        const usageEntries = await Promise.all(allUsers.map(async (user) => {
          try {
            return [user.id, await getAiUsageForUser(user.id)] as const;
          } catch (caught) {
            setError(caught instanceof Error ? caught.message : `Could not load AI usage for ${user.username}.`);
            return [user.id, null] as const;
          }
        }));
        setAiUsage(Object.fromEntries(usageEntries));
        setPromptLimits(Object.fromEntries(usageEntries.map(([userId, usage]) => [userId, String(usage?.totalAvailablePrompt ?? 0)])));
      })
      .catch((caught: unknown) => setError(caught instanceof Error ? caught.message : 'Could not load users.'));
  }
  useEffect(load, []);
  async function change(id: number, changes: { roleId?: number; isActive?: boolean }) {
    setError(null);
    try {
      const updated = await updateUser(id, changes);
      setUsers((current) => current.map((user) => user.id === id ? updated : user));
    } catch (caught) { setError(caught instanceof Error ? caught.message : 'Could not update user.'); }
  }
  async function remove(user: ApiUser) {
    if (!window.confirm(`Delete ${user.username}? This action cannot be undone.`)) return;
    try { await deleteUser(user.id); setUsers((current) => current.filter((item) => item.id !== user.id)); }
    catch (caught) { setError(caught instanceof Error ? caught.message : 'Could not delete user.'); }
  }
  async function savePromptLimit(userId: number) {
    const usage = aiUsage[userId];
    const totalAvailablePrompt = Number(promptLimits[userId]);
    if (!usage || !Number.isInteger(totalAvailablePrompt) || totalAvailablePrompt < 0) return;
    setError(null);
    try {
      const updated = await updateAiUsageLimit(usage.id, totalAvailablePrompt);
      setAiUsage((current) => ({ ...current, [userId]: updated }));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Could not update the AI prompt limit.');
    }
  }
  const roleName = (role: Role) => role.name;
  return (
    <AdminGate>
      <main className="page-shell portal-page">
        <header className="page-heading"><p className="eyebrow">Admin</p><h1>Users &amp; AI usage</h1><p>Manage access, account status, and per-user AI prompt limits.</p></header>
        {error && <div className="inline-alert error-message" role="alert">{error}</div>}
        <div className="table-wrap"><table><thead><tr><th>User</th><th>Email</th><th>Role</th><th>Status</th><th>AI usage</th><th>Actions</th></tr></thead><tbody>
          {users.map((user) => <tr key={user.id}>
            <td><strong>{user.username}</strong></td><td>{user.email}</td>
            <td><select aria-label={`Role for ${user.username}`} value={user.roleId} onChange={(event) => void change(user.id, { roleId: Number(event.target.value) })}>{roles.map((role) => <option key={role.roleId} value={role.roleId}>{roleName(role)}</option>)}</select></td>
            <td><span className={user.isActive ? 'status-pill active' : 'status-pill'}>{user.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>{aiUsage[user.id] ? <div className="ai-usage-cell">
              <span>{aiUsage[user.id]?.totalRequests ?? 0} requests · {aiUsage[user.id]?.totalPromptUsed ?? 0} used</span>
              <div className="ai-limit-control">
                <input aria-label={`Available prompts for ${user.username}`} type="number" min={0} value={promptLimits[user.id] ?? '0'} onChange={(event) => setPromptLimits((current) => ({ ...current, [user.id]: event.target.value }))} />
                <button type="button" onClick={() => void savePromptLimit(user.id)}>Save limit</button>
              </div>
            </div> : <span className="muted-copy">No AI usage record</span>}</td>
            <td className="table-actions"><button type="button" onClick={() => void change(user.id, { isActive: !user.isActive })}>{user.isActive ? 'Deactivate' : 'Reactivate'}</button><button type="button" className="danger-link" onClick={() => void remove(user)}>Delete</button></td>
          </tr>)}
        </tbody></table></div>
      </main>
    </AdminGate>
  );
}

const AdminPages = { Dashboard, Games, GameEditor, Genres, GameTypes, Users };
export default AdminPages;
