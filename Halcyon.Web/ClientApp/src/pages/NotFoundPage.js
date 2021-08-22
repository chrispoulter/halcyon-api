import React from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet';

export const NotFoundPage = () => (
    <>
        <Helmet>
            <title>Page Not Found</title>
        </Helmet>

        <section className="bg-light pt-5 pb-5 mb-3">
            <div className="container pt-5 pb-5">
                <h1 className="display-3">Page Not Found</h1>
                <hr />
                <p className="lead">
                    Sorry, the Page you were looking for could not be found.
                </p>
                <p className="text-end">
                    <Link to="/" className="btn btn-primary btn-lg">
                        Home
                    </Link>
                </p>
            </div>
        </section>
    </>
);
