import { Skeleton } from '@/components/ui/skeleton';

export function UpdateProfileLoading() {
    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Skeleton className="h-14 w-3/4" />
            <Skeleton className="h-14" />

            <div className="space-y-6">
                <Skeleton className="h-14" />

                <div className="flex flex-col gap-6 sm:flex-row">
                    <Skeleton className="h-14 w-full" />
                    <Skeleton className="h-14 w-full" />
                </div>

                <div className="flex gap-2">
                    <Skeleton className="h-14 w-full" />
                    <Skeleton className="h-14 w-full" />
                    <Skeleton className="h-14 w-full" />
                </div>

                <div className="flex flex-col justify-end gap-2 sm:flex-row">
                    <Skeleton className="h-10 w-full sm:w-32" />
                    <Skeleton className="h-10 w-full sm:w-32" />
                </div>
            </div>
        </main>
    );
}
