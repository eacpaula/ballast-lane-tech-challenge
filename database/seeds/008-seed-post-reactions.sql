INSERT INTO post_reactions (
    post_id,
    user_id,
    visitor_identifier,
    reaction_type,
    reaction_date,
    creation_user_id,
    update_user_id
)
VALUES
    (
        (SELECT id FROM posts WHERE title = 'Building a Lightweight Clean Architecture'),
        (SELECT id FROM users WHERE username = 'admin'),
        NULL,
        'like',
        NOW() - INTERVAL '6 days',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    ),
    (
        (SELECT id FROM posts WHERE title = 'Using TDD to Drive Interview-Quality Code'),
        NULL,
        'visitor-demo-001',
        'dislike',
        NOW() - INTERVAL '1 day',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
