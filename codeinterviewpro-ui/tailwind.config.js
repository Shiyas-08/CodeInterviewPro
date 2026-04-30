/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}"
  ],
  theme: {
    extend: {
      colors: {
        accent: {
          DEFAULT: '#2563eb',
          soft:    'rgba(37,99,235,0.12)',
          glow:    'rgba(37,99,235,0.22)',
        },
        glass: {
          white:  'rgba(255,255,255,0.62)',
          border: 'rgba(255,255,255,0.85)',
        }
      },
      fontFamily: {
        sans: ['Inter', '-apple-system', 'BlinkMacSystemFont', 'SF Pro Display', 'ui-sans-serif'],
      },
      borderRadius: {
        '4xl': '2rem',
        '5xl': '2.5rem',
      },
      boxShadow: {
        glass:  '0 20px 60px -8px rgba(100,116,155,0.18), 0 8px 24px -4px rgba(80,100,140,0.10)',
        'glass-hover': '0 28px 72px -8px rgba(80,100,140,0.26), 0 12px 32px -4px rgba(80,100,140,0.15)',
        'glass-inset': '0 1.5px 0px 0px rgba(255,255,255,0.95) inset',
        'btn-blue':    '0 6px 20px rgba(37,99,235,0.35), 0 2px 6px rgba(37,99,235,0.20)',
      },
      backdropBlur: {
        xs:  '4px',
        sm:  '8px',
        md:  '16px',
        lg:  '30px',
        xl:  '40px',
        '2xl': '60px',
      },
      animation: {
        'fade-in':  'vFadeIn  0.45s ease-out both',
        'slide-up': 'vSlideUp 0.50s cubic-bezier(.16,1,.3,1) both',
        'scale-in': 'vScaleIn 0.40s cubic-bezier(.16,1,.3,1) both',
      },
      keyframes: {
        vFadeIn:  { from: { opacity: '0' }, to: { opacity: '1' } },
        vSlideUp: {
          from: { opacity: '0', transform: 'translateY(24px) scale(0.98)' },
          to:   { opacity: '1', transform: 'translateY(0) scale(1)' }
        },
        vScaleIn: {
          from: { opacity: '0', transform: 'scale(0.94)' },
          to:   { opacity: '1', transform: 'scale(1)' }
        },
      }
    },
  },
  plugins: [],
}