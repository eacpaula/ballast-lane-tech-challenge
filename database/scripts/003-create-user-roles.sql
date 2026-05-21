CREATE TABLE IF NOT EXISTS user_roles (
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id INTEGER NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    creation_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    update_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creation_user_id INTEGER NULL REFERENCES users(id),
    update_user_id INTEGER NULL REFERENCES users(id),
    PRIMARY KEY (user_id, role_id)
);
