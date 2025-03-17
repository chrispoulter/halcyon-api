import { ArrowDownWideNarrow } from 'lucide-react';
import { UserSort } from '@/features/user/user-types';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

const sortOptions = [
    {
        value: UserSort.NAME_ASC,
        label: 'Name A-Z',
    },
    {
        value: UserSort.NAME_DESC,
        label: 'Name Z-A',
    },
    {
        value: UserSort.EMAIL_ADDRESS_ASC,
        label: 'Email Address A-Z',
    },
    {
        value: UserSort.EMAIL_ADDRESS_DESC,
        label: 'Email Address Z-A',
    },
];

type SortUsersDropdownProps = {
    sort?: UserSort;
    onChange: (sort: UserSort) => void;
    disabled?: boolean;
};

export function SortUsersDropdown({
    sort,
    onChange,
    disabled,
}: SortUsersDropdownProps) {
    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="secondary" size="icon" disabled={disabled}>
                    <ArrowDownWideNarrow />
                    <span className="sr-only">Toggle sort</span>
                </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="w-56">
                {sortOptions.map(({ label, value }) => (
                    <DropdownMenuItem
                        key={value}
                        disabled={sort === value}
                        onClick={() => onChange(value)}
                    >
                        {label}
                    </DropdownMenuItem>
                ))}
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
