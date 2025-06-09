import { Skeleton } from '@/components/ui/skeleton';

export function ProfileLoading() {
    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Skeleton className="h-14 w-3/4" />

            <Skeleton className="h-10 w-1/2" />
            <Skeleton className="h-36 w-full" />
            <Skeleton className="h-10 w-full sm:w-32" />

            <Skeleton className="h-10 w-1/2" />
            <Skeleton className="h-14" />
            <Skeleton className="h-10 w-full sm:w-32" />

            <Skeleton className="h-10 w-1/2" />
            <Skeleton className="h-14" />
            <Skeleton className="h-10 w-full sm:w-32" />
        </main>
    );
}
