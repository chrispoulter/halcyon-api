import { Skeleton } from '@/components/ui/skeleton';

export function SearchUsersLoading() {
    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Skeleton className="h-14 w-3/4" />
            <Skeleton className="h-14" />
            <Skeleton className="h-10 w-full sm:w-32" />

            <div className="space-y-6">
                <Skeleton className="h-20" />
                <Skeleton className="h-20" />
                <Skeleton className="h-20" />
                <Skeleton className="h-20" />
                <Skeleton className="h-20" />
            </div>

            <div className="flex flex-row justify-center gap-2">
                <Skeleton className="h-10 w-32" />
                <Skeleton className="h-10 w-32" />
            </div>
        </main>
    );
}
