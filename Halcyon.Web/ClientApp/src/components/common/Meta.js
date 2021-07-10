import React from 'react';
import { Helmet } from 'react-helmet';

export const Meta = () => (
    <Helmet defaultTitle="Halcyon" titleTemplate="%s // Halcyon">
        <meta name="description" content="A React web site project template." />
        <meta name="keywords" content="project template, react" />
    </Helmet>
);
