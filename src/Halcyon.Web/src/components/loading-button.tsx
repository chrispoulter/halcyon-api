import { Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';

type LoadingButtonProps = { loading?: boolean } & React.ComponentProps<
    typeof Button
>;

export function LoadingButton({
    loading,
    children,
    disabled,
    ...rest
}: LoadingButtonProps) {
    return (
        <Button {...rest} disabled={disabled || loading}>
            {loading && <Loader2 className="absolute animate-spin" />}
            <span
                className={cn({
                    invisible: loading,
                })}
            >
                {children}
            </span>
        </Button>
    );
}
