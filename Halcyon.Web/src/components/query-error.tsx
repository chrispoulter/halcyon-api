import { LogoutRedirect } from '@/components/logout-redirect';
import { ApiClientError } from '@/lib/api-client';
import { ErrorPage } from '@/error-page';
import { ForbiddenPage } from '@/forbidden-page';
import { NotFoundPage } from '@/not-found-page';

type QueryErrorProps = { error: Error | null };

export function QueryError({ error }: QueryErrorProps) {
    if (error instanceof ApiClientError) {
        switch (error.status) {
            case 401:
                return <LogoutRedirect />;

            case 403:
                return <ForbiddenPage />;

            case 404:
                return <NotFoundPage />;
        }
    }

    return <ErrorPage />;
}
