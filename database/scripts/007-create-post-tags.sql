CREATE TABLE IF NOT EXISTS post_tags (
    post_id INTEGER NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    tag_id INTEGER NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    creation_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    update_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creation_user_id INTEGER NULL REFERENCES users(id),
    update_user_id INTEGER NULL REFERENCES users(id),
    PRIMARY KEY (post_id, tag_id)
);
