import {
    Pagination,
    PaginationContent,
    PaginationItem,
    PaginationNext,
    PaginationPrevious,
} from '@/components/ui/pagination';

type PagerProps = {
    hasPreviousPage: boolean;
    hasNextPage: boolean;
    onPreviousPage: () => void;
    onNextPage: () => void;
    disabled?: boolean;
};

export function Pager({
    hasPreviousPage,
    hasNextPage,
    onPreviousPage,
    onNextPage,
    disabled,
}: PagerProps) {
    if (!hasPreviousPage && !hasNextPage) {
        return null;
    }

    return (
        <Pagination>
            <PaginationContent>
                {hasPreviousPage && (
                    <PaginationItem>
                        <PaginationPrevious
                            disabled={disabled}
                            onClick={() => onPreviousPage()}
                        />
                    </PaginationItem>
                )}
                {hasNextPage && (
                    <PaginationItem>
                        <PaginationNext
                            disabled={disabled}
                            onClick={() => onNextPage()}
                        />
                    </PaginationItem>
                )}
            </PaginationContent>
        </Pagination>
    );
}
