CREATE TABLE IF NOT EXISTS post_reactions (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    post_id INTEGER NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    user_id INTEGER NULL REFERENCES users(id) ON DELETE SET NULL,
    visitor_identifier VARCHAR(200) NULL,
    reaction_type VARCHAR(20) NOT NULL,
    reaction_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creation_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    update_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creation_user_id INTEGER NULL REFERENCES users(id),
    update_user_id INTEGER NULL REFERENCES users(id),
    CONSTRAINT ck_post_reactions_type CHECK (reaction_type IN ('like', 'dislike')),
    CONSTRAINT ck_post_reactions_actor CHECK (user_id IS NOT NULL OR visitor_identifier IS NOT NULL)
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_post_reactions_user
    ON post_reactions(post_id, user_id)
    WHERE user_id IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ux_post_reactions_visitor
    ON post_reactions(post_id, visitor_identifier)
    WHERE visitor_identifier IS NOT NULL;
