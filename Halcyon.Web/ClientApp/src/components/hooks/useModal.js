export const useModal = () => {
    const confirm = () => Promise.resolve(false);

    return { confirm };
};
