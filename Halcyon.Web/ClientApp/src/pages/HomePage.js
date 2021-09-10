import React from 'react';
import { Link } from 'react-router-dom';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import { Hero, Button } from '../components';

export const HomePage = () => (
    <>
        <Hero>
            <h1 className="display-3">Welcome!</h1>
            <hr />
            <p className="lead">
                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam
                semper diam at erat pulvinar, at pulvinar felis blandit.
                Vestibulum volutpat tellus diam, consequat gravida libero
                rhoncus ut. Morbi maximus, leo sit amet vehicula eleifend, nunc
                dui porta orci, quis semper odio felis ut quam.
            </p>
            <p className="text-end">
                <Button to="/register" as={Link} variant="primary" size="lg">
                    Get Started
                </Button>
            </p>
        </Hero>

        <Container className="mb-3">
            <Row className="gx-5">
                <Col lg>
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
                </Col>
                <Col lg>
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
                </Col>
                <Col lg>
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
                </Col>
            </Row>
        </Container>
    </>
);
