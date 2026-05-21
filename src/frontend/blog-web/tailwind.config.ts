import type { Config } from 'tailwindcss'

export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    container: {
      center: true,
      padding: {
        DEFAULT: '1rem',
        sm: '1rem',
        md: '1.5rem',
        lg: '2rem',
      },
      screens: {
        '2xl': '1200px',
      },
    },
    extend: {
      colors: {
        canvas: {
          DEFAULT: '#f8f9fa',
          dim: '#d9dadb',
        },
        navy: '#0f172a',
        ink: {
          DEFAULT: '#191c1d',
          soft: '#584237',
        },
        surface: {
          DEFAULT: '#ffffff',
          low: '#f3f4f5',
          soft: '#f8fafc',
          tint: '#edeeef',
          border: '#e2e8f0',
        },
        brand: {
          DEFAULT: '#f97316',
          strong: '#9d4300',
          contrast: '#ffffff',
          soft: '#ffdbca',
        },
        secondary: {
          DEFAULT: '#565e74',
          soft: '#dae2fd',
        },
      },
      fontFamily: {
        display: ['Geist', 'Inter', 'system-ui', 'sans-serif'],
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['JetBrains Mono', 'ui-monospace', 'SFMono-Regular', 'monospace'],
      },
      spacing: {
        gutter: '24px',
        'gutter-mobile': '16px',
        'stack-sm': '8px',
        'stack-md': '16px',
        'stack-lg': '32px',
        section: '80px',
      },
      maxWidth: {
        content: '1200px',
        measure: '72ch',
      },
      borderRadius: {
        sm: '0.25rem',
        DEFAULT: '0.5rem',
        md: '0.75rem',
        lg: '1rem',
        xl: '1.5rem',
      },
      boxShadow: {
        ambient: '0 4px 6px -1px rgb(0 0 0 / 0.05), 0 2px 4px -2px rgb(0 0 0 / 0.05)',
      },
      letterSpacing: {
        display: '-0.02em',
        headline: '-0.01em',
      },
      lineHeight: {
        14: '3.5rem',
      },
    },
  },
} satisfies Config
