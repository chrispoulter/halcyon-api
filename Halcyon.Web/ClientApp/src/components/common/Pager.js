import React from 'react';
import { Link } from 'react-router-dom';
import Pagination from 'react-bootstrap/Pagination';

export const Pager = ({
    hasNextPage,
    hasPreviousPage,
    onNextPage,
    onPreviousPage
}) => {
    if (!hasNextPage && !hasPreviousPage) {
        return null;
    }

    return (
        <Pagination className="justify-content-center">
            {hasPreviousPage && (
                <Pagination.Item as={Link} onClick={onPreviousPage}>
                    Previous
                </Pagination.Item>
            )}
            {hasNextPage && (
                <Pagination.Item as={Link} onClick={onNextPage}>
                    Next
                </Pagination.Item>
            )}
        </Pagination>
    );
};
