import React from 'react';
import { useTranslation } from 'react-i18next';
import { Pagination, PaginationItem, PaginationLink } from 'reactstrap';

export const Pager = ({
    hasNextPage,
    hasPreviousPage,
    onNextPage,
    onPreviousPage
}) => {
    const { t } = useTranslation();

    if (!hasNextPage && !hasPreviousPage) {
        return null;
    }

    return (
        <Pagination className="d-flex justify-content-center">
            {hasPreviousPage && (
                <PaginationItem>
                    <PaginationLink onClick={onPreviousPage}>
                        {t('components.pager.previous')}
                    </PaginationLink>
                </PaginationItem>
            )}
            {hasNextPage && (
                <PaginationItem>
                    <PaginationLink onClick={onNextPage}>
                        {t('components.pager.next')}
                    </PaginationLink>
                </PaginationItem>
            )}
        </Pagination>
    );
};
