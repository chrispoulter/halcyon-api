import React from 'react';
import { Link } from 'react-router-dom';
import { Container, Jumbotron, Row, Col, Button } from 'reactstrap';

export const HomePage = () => (
    <>
        <Jumbotron>
            <Container>
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
                <p className="text-right">
                    <Button to="/register" color="primary" size="lg" tag={Link}>
                        Get Started
                    </Button>
                </p>
            </Container>
        </Jumbotron>

        <Container>
            <Row className="justify-content-md-center">
                <Col lg={4}>
                    <h2>Fusce condimentum</h2>
                    <hr />
                    <p>
                        In vel tincidunt elit, id pretium massa. Nullam rhoncus
                        orci nisl. Pellentesque in mi et eros porttitor sagittis
                        quis at justo. Sed ac faucibus enim, at tempus enim.
                        Nunc gravida accumsan diam ut maximus. Ut sed tellus
                        odio. N am semper blandit pretium. Suspendisse vitae
                        elit turpis.
                    </p>
                </Col>
                <Col lg={4}>
                    <h2>Fusce condimentum</h2>
                    <hr />
                    <p>
                        In vel tincidunt elit, id pretium massa. Nullam rhoncus
                        orci nisl. Pellentesque in mi et eros porttitor sagittis
                        quis at justo. Sed ac faucibus enim, at tempus enim.
                        Nunc gravida accumsan diam ut maximus. Ut sed tellus
                        odio. N am semper blandit pretium. Suspendisse vitae
                        elit turpis.
                    </p>
                </Col>
                <Col lg={4}>
                    <h2>Fusce condimentum</h2>
                    <hr />
                    <p>
                        In vel tincidunt elit, id pretium massa. Nullam rhoncus
                        orci nisl. Pellentesque in mi et eros porttitor sagittis
                        quis at justo. Sed ac faucibus enim, at tempus enim.
                        Nunc gravida accumsan diam ut maximus. Ut sed tellus
                        odio. N am semper blandit pretium. Suspendisse vitae
                        elit turpis.
                    </p>
                </Col>
            </Row>
        </Container>
    </>
);
