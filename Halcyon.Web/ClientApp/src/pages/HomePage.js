import React from 'react';
import { Link } from 'react-router-dom';

export const HomePage = () => (
    <>
        <section className="bg-light pt-5 pb-5 mb-3">
            <div className="container pt-5 pb-5">
                <h1 className="display-3">Welcome!</h1>
                <hr />
                <p className="lead">
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                    Etiam semper diam at erat pulvinar, at pulvinar felis
                    blandit. Vestibulum volutpat tellus diam, consequat gravida
                    libero rhoncus ut. Morbi maximus, leo sit amet vehicula
                    eleifend, nunc dui porta orci, quis semper odio felis ut
                    quam.
                </p>
                <p className="text-end">
                    <Link to="/register" className="btn btn-primary btn-lg">
                        Get Started
                    </Link>
                </p>
            </div>
        </section>

        <section className="container mb-3">
            <div className="row gx-5">
                <div className="col-lg">
                    <div className="mb-3">
                        <h3>Fusce condimentum</h3>
                        <hr />
                        <p>
                            In vel tincidunt elit, id pretium massa. Nullam
                            rhoncus orci nisl. Pellentesque in mi et eros
                            porttitor sagittis quis at justo. Sed ac faucibus
                            enim, at tempus enim. Nunc gravida accumsan diam ut
                            maximus. Ut sed tellus odio. N am semper blandit
                            pretium. Suspendisse vitae elit turpis.
                        </p>
                    </div>
                </div>
                <div className="col-lg">
                    <div className="mb-3">
                        <h3>Fusce condimentum</h3>
                        <hr />
                        <p>
                            In vel tincidunt elit, id pretium massa. Nullam
                            rhoncus orci nisl. Pellentesque in mi et eros
                            porttitor sagittis quis at justo. Sed ac faucibus
                            enim, at tempus enim. Nunc gravida accumsan diam ut
                            maximus. Ut sed tellus odio. N am semper blandit
                            pretium. Suspendisse vitae elit turpis.
                        </p>
                    </div>
                </div>
                <div className="col-lg">
                    <div className="mb-3">
                        <h3>Fusce condimentum</h3>
                        <hr />
                        <p>
                            In vel tincidunt elit, id pretium massa. Nullam
                            rhoncus orci nisl. Pellentesque in mi et eros
                            porttitor sagittis quis at justo. Sed ac faucibus
                            enim, at tempus enim. Nunc gravida accumsan diam ut
                            maximus. Ut sed tellus odio. N am semper blandit
                            pretium. Suspendisse vitae elit turpis.
                        </p>
                    </div>
                </div>
            </div>
        </section>
    </>
);
