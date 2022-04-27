import React from 'react';
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
                <Pagination.Item onClick={onPreviousPage}>
                    Previous
                </Pagination.Item>
            )}
            {hasNextPage && (
                <Pagination.Item onClick={onNextPage}>Next</Pagination.Item>
            )}
        </Pagination>
    );
};
