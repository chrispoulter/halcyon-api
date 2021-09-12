module.exports = {
    globDirectory: 'build/',
    globPatterns: ['**/*.{json,xml,ico,png,svg,html,js,txt,webmanifest,css}'],
    globIgnores: ['api/**/*', 'swagger/**/*', 'health/**/*'],
    swDest: './build/service-worker.js',
    swSrc: './public/service-worker.js'
};
