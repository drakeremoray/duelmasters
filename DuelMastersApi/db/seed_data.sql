-- Seed data for DuelMasters (Phase 2)
-- Insert a sample player, cards, a deck and deck_cards

INSERT INTO players (username, password_hash, display_name)
VALUES ('seed_player', NULL, 'Seed Player')
ON CONFLICT (username) DO NOTHING;

-- sample cards
INSERT INTO cards (name, card_type, cost, text)
VALUES
  ('Fire Imp', 'Creature', 2, 'Small aggressive creature'),
  ('Water Sprite', 'Creature', 3, 'Defensive creature'),
  ('Lightning Bolt', 'Spell', 1, 'Deal damage to target')
ON CONFLICT (name) DO NOTHING;

-- find seed_player id
-- (psql example: \set pid `select id from players where username='seed_player'`)

-- create a deck for seed_player (if not exists)
INSERT INTO decks (player_id, name)
SELECT p.id, 'Starter Deck' FROM players p WHERE p.username = 'seed_player'
ON CONFLICT DO NOTHING;

-- add cards to the deck (simplified: assumes single deck)
WITH d AS (
  SELECT id FROM decks WHERE name = 'Starter Deck' LIMIT 1
), c AS (
  SELECT id, name FROM cards WHERE name IN ('Fire Imp','Water Sprite','Lightning Bolt')
)
INSERT INTO deck_cards (deck_id, card_id, quantity)
SELECT d.id, c.id, CASE WHEN c.name = 'Lightning Bolt' THEN 3 ELSE 2 END
FROM d, c
ON CONFLICT DO NOTHING;

-- sample match & game state (optional)
INSERT INTO matches (metadata) VALUES ('{"notes":"seed match"}') RETURNING id INTO TEMP TABLE tmp_m;
INSERT INTO game_states (match_id, turn, state)
SELECT id, 0, '{}' FROM tmp_m;
