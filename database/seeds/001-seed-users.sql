INSERT INTO users (
    fullname,
    bio,
    birthday,
    available,
    email,
    username,
    password_hash
)
VALUES
    (
        'Admin User',
        'Local demo administrator account.',
        DATE '1990-01-01',
        TRUE,
        'admin@blogplatform.local',
        'admin',
        crypt('Admin123!', gen_salt('bf'))
    ),
    (
        'Regular User',
        'Local demo author account.',
        DATE '1994-05-14',
        TRUE,
        'user@blogplatform.local',
        'user',
        crypt('User123!', gen_salt('bf'))
    );
