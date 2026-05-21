CREATE TABLE IF NOT EXISTS posts (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    post_category_id INTEGER NOT NULL REFERENCES post_categories(id) ON DELETE RESTRICT,
    title VARCHAR(200) NOT NULL,
    description TEXT NULL,
    content TEXT NOT NULL,
    likes INTEGER NOT NULL DEFAULT 0,
    available BOOLEAN NOT NULL DEFAULT TRUE,
    public_post BOOLEAN NOT NULL DEFAULT TRUE,
    publish_date TIMESTAMPTZ NULL,
    expire_date TIMESTAMPTZ NULL,
    creation_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    update_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creation_user_id INTEGER NULL REFERENCES users(id),
    update_user_id INTEGER NULL REFERENCES users(id)
);

CREATE INDEX IF NOT EXISTS ix_posts_user_id ON posts(user_id);
CREATE INDEX IF NOT EXISTS ix_posts_category_id ON posts(post_category_id);
CREATE INDEX IF NOT EXISTS ix_posts_public_available ON posts(public_post, available);
