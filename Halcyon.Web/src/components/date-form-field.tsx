import {
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '@/components/ui/form';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { currentYear, monthNames } from '@/lib/dates';

type DateFormFieldProps = {
    name: string;
    label: string;
    required?: boolean;
    disabled?: boolean;
    autoComplete?: [string, string, string];
};

export function DateFormField({
    name,
    label,
    required,
    disabled,
    autoComplete,
}: DateFormFieldProps) {
    const [dayAuto, monthAuto, yearAuto] = autoComplete || [];

    return (
        <FormField
            name={name}
            render={({ field: { name, value = '--', onChange } }) => {
                const [year, month, day] = value.split('-');

                function onDayChange(value: string) {
                    onChange(`${year}-${month}-${value}`);
                }

                function onMonthChange(value: string) {
                    onChange(`${year}-${value}-${day}`);
                }

                function onYearChange(value: string) {
                    onChange(`${value}-${month}-${day}`);
                }

                return (
                    <FormItem>
                        <FormLabel>{label}</FormLabel>
                        <div className="flex gap-2">
                            <div className="flex-1">
                                <Select
                                    onValueChange={onDayChange}
                                    defaultValue={day}
                                    required={required}
                                    disabled={disabled}
                                    autoComplete={dayAuto}
                                >
                                    <FormControl>
                                        <SelectTrigger>
                                            <SelectValue placeholder="Day..." />
                                        </SelectTrigger>
                                    </FormControl>
                                    <SelectContent>
                                        {Array.from({ length: 31 }).map(
                                            (_, index) => (
                                                <SelectItem
                                                    key={index}
                                                    value={(index + 1)
                                                        .toString()
                                                        .padStart(2, '0')}
                                                >
                                                    {(index + 1)
                                                        .toString()
                                                        .padStart(2, '0')}
                                                </SelectItem>
                                            )
                                        )}
                                    </SelectContent>
                                </Select>
                            </div>
                            <div className="flex-1">
                                <Select
                                    onValueChange={onMonthChange}
                                    defaultValue={month}
                                    required={required}
                                    disabled={disabled}
                                    autoComplete={monthAuto}
                                >
                                    <FormControl id={`${name}-month`}>
                                        <SelectTrigger>
                                            <SelectValue placeholder="Month..." />
                                        </SelectTrigger>
                                    </FormControl>
                                    <SelectContent>
                                        {monthNames.map((_, index) => (
                                            <SelectItem
                                                key={index}
                                                value={(index + 1)
                                                    .toString()
                                                    .padStart(2, '0')}
                                            >
                                                {monthNames[index]}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                            <div className="flex-1">
                                <Select
                                    onValueChange={onYearChange}
                                    defaultValue={year}
                                    required={required}
                                    disabled={disabled}
                                    autoComplete={yearAuto}
                                >
                                    <FormControl id={`${name}-year`}>
                                        <SelectTrigger>
                                            <SelectValue placeholder="Year..." />
                                        </SelectTrigger>
                                    </FormControl>
                                    <SelectContent>
                                        {Array.from({ length: 120 }).map(
                                            (_, index) => (
                                                <SelectItem
                                                    key={index}
                                                    value={(
                                                        currentYear - index
                                                    ).toString()}
                                                >
                                                    {currentYear - index}
                                                </SelectItem>
                                            )
                                        )}
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>
                        <FormMessage />
                    </FormItem>
                );
            }}
        />
    );
}
