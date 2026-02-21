-- Initial schema for DuelMasters (Phase 1)
-- Run against a Postgres database

CREATE TABLE IF NOT EXISTS players (
  id SERIAL PRIMARY KEY,
  username TEXT NOT NULL UNIQUE,
  password_hash TEXT,
  display_name TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE IF NOT EXISTS cards (
  id SERIAL PRIMARY KEY,
  name TEXT NOT NULL,
  card_type TEXT,
  cost INT,
  text TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE IF NOT EXISTS decks (
  id SERIAL PRIMARY KEY,
  player_id INT NOT NULL REFERENCES players(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE IF NOT EXISTS deck_cards (
  id SERIAL PRIMARY KEY,
  deck_id INT NOT NULL REFERENCES decks(id) ON DELETE CASCADE,
  card_id INT NOT NULL REFERENCES cards(id) ON DELETE CASCADE,
  quantity INT DEFAULT 1
);

CREATE TABLE IF NOT EXISTS matches (
  id SERIAL PRIMARY KEY,
  started_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  finished_at TIMESTAMP WITH TIME ZONE,
  metadata JSONB
);

CREATE TABLE IF NOT EXISTS game_states (
  id SERIAL PRIMARY KEY,
  match_id INT NOT NULL REFERENCES matches(id) ON DELETE CASCADE,
  turn INT DEFAULT 0,
  state JSONB NOT NULL,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);
