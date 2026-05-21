INSERT INTO roles (
    title,
    description,
    creation_user_id,
    update_user_id
)
VALUES
    (
        'Administrator',
        'Can manage post categories and other admin-only blog content tasks.',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    ),
    (
        'User',
        'Standard authenticated blog contributor.',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
