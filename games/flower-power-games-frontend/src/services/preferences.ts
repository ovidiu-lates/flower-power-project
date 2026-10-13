import { ApiError, apiRequest } from '../lib/api';

export type Preference = {
  id?: number;
  minBudget: number;
  maxBudget: number;
  minPlayTime: number;
  maxPlayTime: number;
  minPlayers: number;
  maxPlayers: number;
  minimumAge: number;
  genreIds: number[];
  typeIds: number[];
};

export type PreferenceInput = Omit<Preference, 'id'>;

export async function getMyPreference(): Promise<Preference | null> {
  try {
    const preference = await apiRequest<Preference | null>('/UserPreference');
    if (!preference) return null;
    return {
      ...preference,
      genreIds: preference.genreIds ?? [],
      typeIds: preference.typeIds ?? [],
    };
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null;
    throw error;
  }
}

export const createPreference = (preference: PreferenceInput) =>
  apiRequest<Preference>('/UserPreference', { method: 'POST', body: JSON.stringify(preference) });

export const updatePreference = (preference: PreferenceInput) =>
  apiRequest<Preference>('/UserPreference', { method: 'PUT', body: JSON.stringify(preference) });

export async function upsertPreference(preference: PreferenceInput): Promise<Preference> {
  const existingPreference = await getMyPreference();

  if (existingPreference) {
    try {
      return await updatePreference(preference);
    } catch (error) {
      if (error instanceof ApiError && error.status === 404) {
        return createPreference(preference);
      }
      throw error;
    }
  }

  try {
    return await createPreference(preference);
  } catch (error) {
    if (error instanceof ApiError && error.status === 409) {
      return updatePreference(preference);
    }
    throw error;
  }
}
