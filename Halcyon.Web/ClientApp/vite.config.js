import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import { execSync } from 'child_process';

const generateCerts = () => {
    const baseFolder =
        process.env.APPDATA !== undefined && process.env.APPDATA !== ''
            ? `${process.env.APPDATA}/ASP.NET/https`
            : `${process.env.HOME}/.aspnet/https`;

    const certificateArg = process.argv
        .map(arg => arg.match(/--name=(?<value>.+)/i))
        .filter(Boolean)[0];

    const certificateName = certificateArg
        ? certificateArg.groups.value
        : process.env.npm_package_name;

    if (!certificateName) {
        console.error(
            'Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.'
        );
        process.exit(-1);
    }

    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        execSync(
            [
                'dotnet',
                'dev-certs',
                'https',
                '--export-path',
                certFilePath,
                '--format',
                'Pem',
                '--no-password'
            ].join(' '),
            { stdio: 'inherit' }
        );
    }

    return {
        cert: certFilePath,
        key: keyFilePath
    };
};

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        plugins: [react()],
        resolve: {
            alias: {
                '@/components': path.resolve(__dirname, './src/components'),
                '@/hooks': path.resolve(__dirname, './src/hooks'),
                '@/pages': path.resolve(__dirname, './src/pages'),
                '@/redux': path.resolve(__dirname, './src/redux'),
                '@/styles': path.resolve(__dirname, './src/styles'),
                '@/utils': path.resolve(__dirname, './src/utils')
            }
        },
        server: {
            port: 44485,
            strictPort: true,
            https: generateCerts(),
            proxy: {
                '^/(api|swagger|health)/*': {
                    secure: false,
                    changeOrigin: true,
                    target: env.ASPNETCORE_HTTPS_PORT
                        ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
                        : env.ASPNETCORE_URLS
                        ? env.ASPNETCORE_URLS.split(';')[0]
                        : 'http://localhost:56453'
                }
            }
        }
    };
});
