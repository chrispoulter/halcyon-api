import { config } from '@/lib/config';
import { currentYear } from '@/lib/dates';

export function Footer() {
    return (
        <footer className="mt-6 border-t">
            <div className="mx-auto flex max-w-screen-sm flex-col justify-between gap-2 p-6 text-center sm:flex-row sm:text-left">
                <div className="text-sm leading-none font-medium">
                    &copy;{' '}
                    <a
                        href="http://www.chrispoulter.com"
                        className="text-primary font-medium underline underline-offset-4"
                    >
                        Chris Poulter
                    </a>{' '}
                    {currentYear}
                </div>
                <div className="text-sm leading-none font-medium">
                    v{config.VERSION}
                </div>
            </div>
        </footer>
    );
}
