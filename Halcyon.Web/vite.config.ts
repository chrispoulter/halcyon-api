import { defineConfig, loadEnv } from 'vite';
import path from 'path';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        plugins: [react(), tailwindcss()],
        define: {
            'import.meta.env.npm_package_version': JSON.stringify(
                env.npm_package_version
            ),
        },
        resolve: {
            alias: {
                '@': path.resolve(__dirname, './src'),
            },
        },
        server: {
            port: parseInt(env.VITE_PORT),
            proxy: {
                '/api': {
                    target:
                        env.services__api__https__0 ||
                        env.services__api__http__0,
                    changeOrigin: true,
                    rewrite: (path) => path.replace(/^\/api/, ''),
                    secure: false,
                },
            },
        },
        build: {
            outDir: 'dist',
            rollupOptions: {
                input: './index.html',
            },
        },
    };
});
