// Enums matching the backend
export enum Sport {
  General = 'General',
  Football = 'Football',
  Basketball = 'Basketball',
  Tennis = 'Tennis',
  Padel = 'Padel',
  Volleyball = 'Volleyball',
  Handball = 'Handball',
  Athletics = 'Athletics',
  Gym = 'Gym',
  Swimming = 'Swimming',
  Cycling = 'Cycling',
  Other = 'Other',
}

export enum Difficulty {
  Beginner = 'Beginner',
  Intermediate = 'Intermediate',
  Advanced = 'Advanced',
}

export enum MarketplaceItemType {
  Objective = 'Objective',
  Exercise = 'Exercise',
  Workout = 'Workout',
  TrainingPlan = 'TrainingPlan',
  Itinerary = 'Itinerary',
}

export enum MarketplaceFilter {
  MostPopular = 'MostPopular',
  TopRated = 'TopRated',
  Newest = 'Newest',
}

// Interfaces matching backend DTOs
export interface MarketplaceRating {
  id: string;
  ratedBySubscriptionId: string;
  stars: number;
  comment?: string;
  createdAt: string;
}

export interface MarketplaceItem {
  id: string;
  type: MarketplaceItemType;
  sport: Sport;
  sourceEntityId?: string;
  name: string;
  description: string;
  isSystemOfficial: boolean;
  averageRating: number;
  totalRatings: number;
  totalDownloads: number;
  totalViews: number;
  publishedAt: string;
  ratings: MarketplaceRating[];
}

export interface ItineraryItem {
  marketplaceItemId: string;
  name: string;
  type: MarketplaceItemType;
  order: number;
}

export interface Itinerary {
  id: string;
  name: string;
  description: string;
  sport: Sport;
  level: Difficulty;
  isActive: boolean;
  items: ItineraryItem[];
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}