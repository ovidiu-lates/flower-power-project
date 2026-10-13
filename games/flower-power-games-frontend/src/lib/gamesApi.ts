import type { Difficulty, Game, GameInsight, PaginatedGamesResponse, Review } from "../types/game";
import { API_BASE_URL } from "./api";

type GetGamesParams = {
  search?: string;
  players?: string;
  maxTime?: string;
  difficulty?: Difficulty | "";
  sort?: "rating" | "name";
  token?: string;
};

function authHeaders(token?: string): HeadersInit | undefined {
  return token ? { Authorization: `Bearer ${token}` } : undefined;
}

export async function getGames({
  search,
  players,
  maxTime,
  difficulty,
  sort,
  token,
}: GetGamesParams): Promise<PaginatedGamesResponse> {
  const headers = authHeaders(token);
  const [gamesResponse, genresResponse, typesResponse] = await Promise.all([
    fetch(`${API_BASE_URL}/Games`, { headers }),
    fetch(`${API_BASE_URL}/Genres`, { headers }),
    fetch(`${API_BASE_URL}/GameTypes`, { headers }),
  ]);

  if (!gamesResponse.ok || !genresResponse.ok || !typesResponse.ok) {
    throw new Error("Could not load the game catalog from the API.");
  }

  const games = await gamesResponse.json() as Omit<Game, "tags">[];
  const genres = await genresResponse.json() as Array<{ id: number; name: string }>;
  const types = await typesResponse.json() as Array<{ id: number; name: string }>;
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
      ]
        .filter((name): name is string => Boolean(name)),
    }))
    .filter((game) => !search || game.name.toLowerCase().includes(search.toLowerCase()))
    .filter((game) => requestedPlayers === null || (
      game.minPlayers <= requestedPlayers && game.maxPlayers >= requestedPlayers
    ))
    .filter((game) => requestedMaxTime === null || game.playTimeMinutes <= requestedMaxTime)
    .filter((game) => !difficulty || getDifficulty(game.learningTimeMinutes) === difficulty)
    .sort((left, right) => sort === "name"
      ? left.name.localeCompare(right.name)
      : right.rating - left.rating);

  return {
    items,
    page: 1,
    page_size: items.length,
    total: items.length,
    total_pages: 1,
  };
}

function getDifficulty(learningTimeMinutes: number): Difficulty {
  if (learningTimeMinutes <= 15) return "easy";
  if (learningTimeMinutes <= 30) return "medium";
  return "hard";
}

export async function getGameInsight(
  gameId: number,
  token?: string,
): Promise<GameInsight | null> {
  const response = await fetch(`${API_BASE_URL}/games/${gameId}/insight`, {
    headers: authHeaders(token),
  });

  if (response.status === 404) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Failed to fetch game insight: ${response.status}`);
  }

  return response.json();
}

export async function getGameReviews(gameId: string, token?: string): Promise<Review[]> {
  const response = await fetch(`${API_BASE_URL}/games/${gameId}/reviews`, {
    headers: authHeaders(token),
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch game reviews: ${response.status}`);
  }

  return response.json();
}