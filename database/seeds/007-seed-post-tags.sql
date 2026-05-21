INSERT INTO post_tags (
    post_id,
    tag_id,
    creation_user_id,
    update_user_id
)
VALUES
    (
        (SELECT id FROM posts WHERE title = 'Building a Lightweight Clean Architecture'),
        (SELECT id FROM tags WHERE title = 'clean-architecture'),
        (SELECT id FROM users WHERE username = 'user'),
        (SELECT id FROM users WHERE username = 'user')
    ),
    (
        (SELECT id FROM posts WHERE title = 'Using TDD to Drive Interview-Quality Code'),
        (SELECT id FROM tags WHERE title = 'tdd'),
        (SELECT id FROM users WHERE username = 'admin'),
        (SELECT id FROM users WHERE username = 'admin')
    );
