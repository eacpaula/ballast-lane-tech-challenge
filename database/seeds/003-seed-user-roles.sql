INSERT INTO user_roles (
    user_id,
    role_id,
    creation_user_id,
    update_user_id
)
VALUES
    (
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM roles WHERE title = 'Administrator'),
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    ),
    (
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM roles WHERE title = 'User'),
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
