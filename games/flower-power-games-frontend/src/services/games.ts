import type { Difficulty, Game, GameInsight, PaginatedGamesResponse, Review } from '../types/game';
import { ApiError, apiRequest } from '../lib/api';
import { getGameTypes } from './gameTypes';
import { getGenres } from './genres';

export type GetGamesParams = {
  search?: string;
  players?: string;
  maxTime?: string;
  difficulty?: Difficulty | '';
  sort?: 'rating' | 'name' | 'popular' | 'new';
};

export async function getGames({
  search,
  players,
  maxTime,
  difficulty,
  sort,
}: GetGamesParams = {}): Promise<PaginatedGamesResponse> {
  const [games, genres, types] = await Promise.all([
    apiRequest<Array<Omit<Game, 'tags'>>>('/Games'),
    getGenres(),
    getGameTypes(),
  ]);
  const genreNames = new Map(genres.map((item) => [item.id, item.name]));
  const typeNames = new Map(types.map((item) => [item.id, item.name]));
  const requestedPlayers = players ? Number(players) : null;
  const requestedMaxTime = maxTime ? Number(maxTime) : null;
  const items = games
    .map((game) => ({
      ...game,
      tags: [
        ...game.genreIds.map((id) => genreNames.get(id)),
        ...game.typeIds.map((id) => typeNames.get(id)),
      ].filter((name): name is string => Boolean(name)),
    }))
    .filter((game) => !search || game.name.toLowerCase().includes(search.toLowerCase()))
    .filter((game) => requestedPlayers === null
      || (game.minPlayers <= requestedPlayers && game.maxPlayers >= requestedPlayers))
    .filter((game) => requestedMaxTime === null || game.playTimeMinutes <= requestedMaxTime)
    .filter((game) => !difficulty || getDifficulty(game.learningTimeMinutes) === difficulty)
    .sort((left, right) => {
      if (sort === 'name') return left.name.localeCompare(right.name);
      if (sort === 'new') return right.id - left.id;
      return right.rating - left.rating;
    });

  return {
    items,
    page: 1,
    page_size: items.length,
    total: items.length,
    total_pages: 1,
  };
}

function getDifficulty(learningTimeMinutes: number): Difficulty {
  if (learningTimeMinutes <= 15) return 'easy';
  if (learningTimeMinutes <= 30) return 'medium';
  return 'hard';
}

export const getGameById = (id: number) => apiRequest<Omit<Game, 'tags'>>(`/Games/${id}`);

export const createGame = (game: Omit<Game, 'id' | 'rating' | 'tags'>) =>
  apiRequest<Game>('/Games', {
    method: 'POST',
    body: JSON.stringify({ ...game, rating: 0 }),
  });

export const updateGame = (id: number, game: Omit<Game, 'id' | 'rating' | 'tags'>) =>
  apiRequest<Game>(`/Games/${id}`, {
    method: 'PUT',
    body: JSON.stringify({ ...game, id, rating: 0 }),
  });

export const deleteGame = (id: number) =>
  apiRequest<void>(`/Games/${id}`, { method: 'DELETE' });

export async function getGameInsight(gameId: number): Promise<GameInsight | null> {
  try {
    return await apiRequest<GameInsight>(`/games/${gameId}/insight`);
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null;
    throw error;
  }
}

export const getGameReviews = (gameId: number) =>
  apiRequest<Review[]>(`/games/${gameId}/reviews`);
