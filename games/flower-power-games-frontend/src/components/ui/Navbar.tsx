import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import type { User } from '../../types/user';

type NavbarProps = {
  user: User | null;
  onSignOut: () => void;
};

export default function Navbar({ user, onSignOut }: NavbarProps) {
  const [profileOpen, setProfileOpen] = useState(false);
  const navigate = useNavigate();
  const initials = user?.username.slice(0, 2).toUpperCase() ?? '✿';

  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <Link to="/" className="navbar-logo" aria-label="Flower Power Games home">
          <span className="navbar-logo-mark">✿</span>
          <span>
            Flower Power<br />
            <small>Games - AI Review</small>
          </span>
        </Link>
        <div className="navbar-links" aria-label="Primary navigation">
          <Link className="navbar-link" to="/discover">Discover</Link>
          {user && (
            <>
              <Link className="navbar-link" to="/me/favorites">My library</Link>
              <Link className="navbar-link" to="/me/preferences">Preferences &amp; AI</Link>
              {user.role.toLowerCase().includes('admin') && (
                <>
                  <Link className="navbar-link" to="/admin/games">Games</Link>
                  <Link className="navbar-link" to="/admin/genres">Genres</Link>
                  <Link className="navbar-link" to="/admin/game-types">Game types</Link>
                  <Link className="navbar-link" to="/admin/users">Users</Link>
                </>
              )}
            </>
          )}
        </div>
        <span className="navbar-spacer" />
        {user ? (
          <div className="profile-menu">
            <button
              type="button"
              onClick={() => setProfileOpen((open) => !open)}
              className="profile-button"
              aria-expanded={profileOpen}
              aria-label="Open profile menu"
            >
              {initials}
            </button>
            {profileOpen && (
              <div className="profile-dropdown">
                <div className="profile-summary">
                  <p>{user.username}</p>
                  <small>{user.email}</small>
                </div>
                <Link to={`/users/${encodeURIComponent(user.username)}`} onClick={() => setProfileOpen(false)}>Profile</Link>
                <Link to="/me/edit" onClick={() => setProfileOpen(false)}>Edit profile</Link>
                {user.role.toLowerCase().includes('admin') && (
                  <Link to="/admin" onClick={() => setProfileOpen(false)}>Admin dashboard</Link>
                )}
                <button
                  type="button"
                  onClick={() => {
                    setProfileOpen(false);
                    onSignOut();
                    navigate('/auth/login');
                  }}
                >
                  Sign out
                </button>
              </div>
            )}
          </div>
        ) : (
          <Link className="button button-small" to="/auth/login">Log in</Link>
        )}
      </div>
    </nav>
  );
}
