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
    expire_date,
    creation_user_id,
    update_user_id
)
VALUES
    (
        -- Active post: explicit publish_date in the past, no expiration (always visible)
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM post_categories WHERE title = 'Architecture'),
        'Building a Lightweight Clean Architecture',
        'Why explicit boundaries matter in a small technical challenge.',
        'A lightweight architecture keeps business rules testable without adding enterprise ceremony.',
        1,
        TRUE,
        TRUE,
        NOW() - INTERVAL '7 days',
        NULL,
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user')
    ),
    (
        -- Active post: explicit publish_date in the past, no expiration (always visible)
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM post_categories WHERE title = 'Testing'),
        'Using TDD to Drive Interview-Quality Code',
        'A practical note on writing small, reviewable increments.',
        'Test-first delivery works best when each slice proves one behavior clearly before implementation expands.',
        1,
        TRUE,
        TRUE,
        NOW() - INTERVAL '2 days',
        NULL,
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    ),
    (
        -- Always-visible post: no publish_date, no expire_date (null dates = no window constraint)
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM post_categories WHERE title = 'Architecture'),
        'Clean Architecture Without the Ceremony',
        'Keeping layers clear without requiring a framework to enforce them.',
        'Explicit dependencies between layers make architecture visible in the code itself, not just in diagrams.',
        0,
        TRUE,
        TRUE,
        NULL,
        NULL,
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user')
    ),
    (
        -- Scheduled post: publish_date 7 days in the future — not yet publicly visible
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM post_categories WHERE title = 'Testing'),
        'Upcoming: Advanced Repository Testing Patterns',
        'A look at testing raw SQL repositories against a real PostgreSQL instance.',
        'This post is scheduled for future publication and should not appear in anonymous listings.',
        0,
        TRUE,
        TRUE,
        NOW() + INTERVAL '7 days',
        NULL,
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user')
    ),
    (
        -- Expired post: expire_date in the past — no longer publicly visible
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM post_categories WHERE title = 'Architecture'),
        'Archived: Early Design Decisions',
        'Notes from the initial design session — now superseded by later decisions.',
        'This post has passed its expiration date and should not appear in anonymous listings.',
        0,
        TRUE,
        TRUE,
        NOW() - INTERVAL '30 days',
        NOW() - INTERVAL '1 day',
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
