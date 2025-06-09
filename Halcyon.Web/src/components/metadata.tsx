type MetadataProps = {
    title: string;
};

export const Metadata = ({ title }: MetadataProps) => {
    return <title>{`${title} // Halcyon`}</title>;
};
