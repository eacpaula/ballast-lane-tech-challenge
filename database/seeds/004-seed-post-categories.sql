INSERT INTO post_categories (
    title,
    description,
    available,
    creation_user_id,
    update_user_id
)
VALUES
    (
        'Architecture',
        'Posts about software architecture, design trade-offs, and structure.',
        TRUE,
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    ),
    (
        'Testing',
        'Posts about quality, automated tests, and validation strategy.',
        TRUE,
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
