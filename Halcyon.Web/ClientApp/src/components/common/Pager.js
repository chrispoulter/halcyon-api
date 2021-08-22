import React from 'react';
import { Link } from 'react-router-dom';

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
        <ul className="pagination justify-content-center">
            {hasPreviousPage && (
                <li className="page-item">
                    <Link className="page-link" onClick={onPreviousPage}>
                        Previous
                    </Link>
                </li>
            )}
            {hasNextPage && (
                <li className="page-item">
                    <Link className="page-link" onClick={onNextPage}>
                        Next
                    </Link>
                </li>
            )}
        </ul>
    );
};
