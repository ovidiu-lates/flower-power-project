import { useState } from 'react';
import {
  BrowserRouter,
  Link,
  Navigate,
  Outlet,
  Route,
  Routes,
} from 'react-router-dom';
import Navbar from './components/ui/Navbar';
import AdminPages from './pages/AdminPages';
import Discover from './pages/Discover';
import Login from './pages/Login';
import PortalPages from './pages/PortalPages';
import { clearAccessToken } from './lib/api';
import { getStoredCurrentUser } from './services/auth';
import type { User } from './types/user';

function AppLayout() {
  const [user, setUser] = useState<User | null>(() => getStoredCurrentUser());

  return (
    <>
      <Navbar
        user={user}
        onSignOut={() => {
          clearAccessToken();
          localStorage.removeItem('refresh_token');
          setUser(null);
        }}
      />
      <Outlet context={{ user, setUser }} />
      <footer className="site-footer page-shell">
        <span>Flower Power Games</span>
        <nav aria-label="Footer">
          <Link to="/about">About</Link>
          <Link to="/terms">Terms</Link>
          <Link to="/privacy">Privacy</Link>
        </nav>
      </footer>
    </>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<PortalPages.Home />} />
          <Route path="/discover" element={<Discover />} />
          <Route path="/games/:id" element={<PortalPages.GameDetail />} />
          <Route path="/users/:username" element={<PortalPages.Profile />} />
          <Route path="/me/edit" element={<PortalPages.EditProfile />} />
          <Route path="/me/favorites" element={<PortalPages.Favorites />} />
          <Route path="/me/preferences" element={<PortalPages.Preferences />} />
          <Route path="/ai" element={<PortalPages.Preferences />} />
          <Route path="/admin" element={<AdminPages.Dashboard />} />
          <Route path="/admin/games" element={<AdminPages.Games />} />
          <Route path="/admin/games/new" element={<AdminPages.GameEditor />} />
          <Route path="/admin/games/:id/edit" element={<AdminPages.GameEditor />} />
          <Route path="/admin/genres" element={<AdminPages.Genres />} />
          <Route path="/admin/game-types" element={<AdminPages.GameTypes />} />
          <Route path="/admin/users" element={<AdminPages.Users />} />
          <Route path="/about" element={<PortalPages.StaticPage page="About" />} />
          <Route path="/terms" element={<PortalPages.StaticPage page="Terms" />} />
          <Route path="/privacy" element={<PortalPages.StaticPage page="Privacy" />} />
          <Route path="/auth/login" element={<Login mode="login" />} />
          <Route path="/auth/register" element={<Login mode="register" />} />
          <Route path="/login" element={<Navigate to="/auth/login" replace />} />
          <Route path="/register" element={<Navigate to="/auth/register" replace />} />
          <Route path="/404" element={<PortalPages.NotFound />} />
          <Route path="*" element={<PortalPages.NotFound />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
