import {
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';

type TextFormFieldProps = {
    name: string;
    label: string;
    type?: string;
    maxLength?: number;
    autoComplete?: string;
    required?: boolean;
    disabled?: boolean;
    className?: string;
};

export function TextFormField({
    name,
    label,
    className,
    ...props
}: TextFormFieldProps) {
    return (
        <FormField
            name={name}
            render={({ field }) => (
                <FormItem className={className}>
                    <FormLabel>{label}</FormLabel>
                    <FormControl>
                        <Input {...field} {...props} />
                    </FormControl>
                    <FormMessage />
                </FormItem>
            )}
        />
    );
}
