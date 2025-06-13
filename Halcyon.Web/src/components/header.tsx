import { Link } from 'react-router';
import { ModeToggle } from '@/components/mode-toggle';
import { MainNav } from '@/components/main-nav';
import { UserNav } from '@/components/user-nav';

export function Header() {
    return (
        <header className="mb-6 border-b">
            <div className="mx-auto flex max-w-screen-sm items-center gap-2 px-6 py-4 sm:px-0">
                <div className="flex items-center gap-2">
                    <Link
                        to="/"
                        className="scroll-m-20 text-xl font-semibold tracking-tight"
                    >
                        Halcyon
                    </Link>
                </div>

                <div className="ml-auto flex items-center gap-2">
                    <MainNav />
                    <ModeToggle />
                    <UserNav />
                </div>
            </div>
        </header>
    );
}
