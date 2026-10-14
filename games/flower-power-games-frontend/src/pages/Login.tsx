import { useState, type FormEvent } from 'react';
import { Link, useLocation, useNavigate, useOutletContext } from 'react-router-dom';
import { ApiError } from '../lib/api';
import { getStoredCurrentUser, login, register, saveSession } from '../services/auth';
import type { User } from '../types/user';

export default function Login({ mode }: { mode: 'login' | 'register' }) {
  const { setUser } = useOutletContext<{ setUser: (user: User | null) => void }>();
  const [values, setValues] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const registered = mode === 'login'
    && (location.state as { registered?: boolean } | null)?.registered;

  function update(field: keyof typeof values, value: string) {
    setValues((current) => ({ ...current, [field]: value }));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    if (mode === 'register' && values.password !== values.confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setLoading(true);
    try {
      if (mode === 'login') {
        const response = await login({
          emailOrUsername: values.username,
          password: values.password,
        });
        saveSession(response);
        setUser(getStoredCurrentUser());
        navigate('/discover');
      } else {
        await register({
          username: values.username,
          email: values.email,
          password: values.password,
          confirmPassword: values.confirmPassword,
          fullName: values.username,
        });
        navigate('/auth/login', { state: { registered: true } });
      }
    } catch (caught) {
      setError(caught instanceof Error || caught instanceof ApiError
        ? caught.message
        : 'Request failed. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="login-page">
      <div className="login-container">
        <Link to="/" className="login-brand">
          <span className="login-brand-icon">✿</span>
          <span>
            <strong className="login-brand-name">Flower Power Games</strong>
            <small className="login-brand-subtitle">Find your next favorite game</small>
          </span>
        </Link>
        <section className="login-card">
          <h1 className="login-title">{mode === 'login' ? 'Welcome back' : 'Create your account'}</h1>
          <p className="login-description">
            {mode === 'login'
              ? 'Sign in to save games and get personal recommendations.'
              : 'Join the community and start building your game library.'}
          </p>
          {registered && <div className="login-message" role="status">Account created successfully. You can now log in.</div>}
          {error && <div className="login-error" role="alert">{error}</div>}
          <form className="login-form" onSubmit={handleSubmit}>
            <label className="login-field">
              <span>{mode === 'login' ? 'Email or username' : 'Username'}</span>
              <input
                required
                autoComplete="username"
                value={values.username}
                onChange={(event) => update('username', event.target.value)}
              />
            </label>
            {mode === 'register' && (
              <label className="login-field">
                <span>Email</span>
                <input
                  required
                  type="email"
                  autoComplete="email"
                  value={values.email}
                  onChange={(event) => update('email', event.target.value)}
                />
              </label>
            )}
            <label className="login-field">
              <span>Password</span>
              <input
                required
                type="password"
                minLength={8}
                autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
                value={values.password}
                onChange={(event) => update('password', event.target.value)}
              />
            </label>
            {mode === 'register' && (
              <label className="login-field">
                <span>Confirm password</span>
                <input
                  required
                  type="password"
                  minLength={8}
                  autoComplete="new-password"
                  value={values.confirmPassword}
                  onChange={(event) => update('confirmPassword', event.target.value)}
                />
              </label>
            )}
            <button className="login-submit" type="submit" disabled={loading}>
              {loading ? 'Please wait…' : mode === 'login' ? 'Log in' : 'Create account'}
            </button>
          </form>
          <p className="login-footer">
            {mode === 'login' ? 'New to Flower Power?' : 'Already have an account?'}{' '}
            <Link to={mode === 'login' ? '/auth/register' : '/auth/login'}>
              {mode === 'login' ? 'Create an account' : 'Log in'}
            </Link>
          </p>
        </section>
      </div>
    </main>
  );
}
