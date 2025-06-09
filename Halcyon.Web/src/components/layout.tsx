import { Outlet } from 'react-router';
import { Header } from '@/components/header';
import { Footer } from '@/components/footer';

export function Layout() {
    return (
        <>
            <Header />
            <Outlet />
            <Footer />
        </>
    );
}
