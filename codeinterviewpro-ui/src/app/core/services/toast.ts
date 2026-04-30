export interface ToastOptions {
  description?: string;
  position?: 'top-left' | 'top-center' | 'top-right' | 'bottom-left' | 'bottom-center' | 'bottom-right';
  duration?: number;
}

export interface ToastPromiseOptions<T> {
  loading: string;
  success: (data: T) => string;
  error: string | ((err: any) => string);
}

const dispatch = (type: string, message: string, options?: ToastOptions) => {
  window.dispatchEvent(new CustomEvent('app-toast', {
    detail: { id: Math.random().toString(36).substring(2, 9), type, message, options }
  }));
};

export const toast = Object.assign(
  (message: string, options?: ToastOptions) => dispatch('default', message, options),
  {
    success: (message: string, options?: ToastOptions) => dispatch('success', message, options),
    error: (message: string, options?: ToastOptions) => dispatch('error', message, options),
    warning: (message: string, options?: ToastOptions) => dispatch('warning', message, options),
    info: (message: string, options?: ToastOptions) => dispatch('info', message, options),
    promise: <T>(promiseFn: () => Promise<T>, options: ToastPromiseOptions<T>) => {
      const id = Math.random().toString(36).substring(2, 9);
      window.dispatchEvent(new CustomEvent('app-toast-promise-start', {
        detail: { id, message: options.loading }
      }));

      promiseFn().then(
        (data) => {
          window.dispatchEvent(new CustomEvent('app-toast-promise-resolve', {
            detail: { id, type: 'success', message: options.success(data) }
          }));
        },
        (err) => {
          const errMsg = typeof options.error === 'function' ? options.error(err) : options.error;
          window.dispatchEvent(new CustomEvent('app-toast-promise-resolve', {
            detail: { id, type: 'error', message: errMsg }
          }));
        }
      );
    }
  }
);
