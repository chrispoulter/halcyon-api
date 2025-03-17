import { Link } from 'react-router';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';

export function ForbiddenPage() {
    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Forbidden" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl">
                Forbidden
            </h1>

            <p className="leading-7">
                Sorry, you do not have access to this resource.
            </p>

            <Button asChild className="w-full sm:w-auto">
                <Link to="/">Home</Link>
            </Button>
        </main>
    );
}
