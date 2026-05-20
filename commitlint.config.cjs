module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    // Format: type(scope): subject
    'type-enum': [
      2,
      'always',
      [
        'feat',
        'fix',
        'docs',
        'style',
        'refactor',
        'test',
        'chore',
        'build',
        'ci',
        'perf',
        'revert'
      ]
    ],

    'scope-enum': [
      2,
      'always',
      [
        'repo',
        'api',
        'application',
        'domain',
        'infrastructure',
        'frontend',
        'database',
        'tests',
        'docs',
        'config',
        'tools'
      ]
    ],

    // Require lowercase type and scope
    'type-case': [2, 'always', 'lower-case'],
    'scope-case': [2, 'always', 'lower-case'],

    // Require a subject
    'subject-empty': [2, 'never'],

    // Limit the subject length
    'subject-max-length': [2, 'always', 72],

    // Limit the full header length: type(scope): subject
    'header-max-length': [2, 'always', 100],

    // Optional: do not force subject case
    'subject-case': [0]
  }
};