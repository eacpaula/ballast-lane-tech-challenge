INSERT INTO tags (
    title,
    created_by_user_id,
    creation_user_id,
    update_user_id
)
VALUES
    (
        'clean-architecture',
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user')
    ),
    (
        'tdd',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
