INSERT INTO posts (
    user_id,
    post_category_id,
    title,
    description,
    content,
    likes,
    available,
    public_post,
    publish_date,
    creation_user_id,
    update_user_id
)
VALUES
    (
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM post_categories WHERE title = 'Architecture'),
        'Building a Lightweight Clean Architecture',
        'Why explicit boundaries matter in a small technical challenge.',
        'A lightweight architecture keeps business rules testable without adding enterprise ceremony.',
        1,
        TRUE,
        TRUE,
        NOW() - INTERVAL '7 days',
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user')
    ),
    (
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM post_categories WHERE title = 'Testing'),
        'Using TDD to Drive Interview-Quality Code',
        'A practical note on writing small, reviewable increments.',
        'Test-first delivery works best when each slice proves one behavior clearly before implementation expands.',
        1,
        TRUE,
        TRUE,
        NOW() - INTERVAL '2 days',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
