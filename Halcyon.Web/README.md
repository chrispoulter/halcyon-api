# Halcyon Web

A react web project template 👷 Built with a sense of peace and tranquillity 🙏

## Features

- Vite
  [https://vite.dev/](https://vite.dev/)
- React
  [https://react.dev/](https://react.dev/)
- React Router
  [https://reactrouter.com/](https://reactrouter.com/)
- TanStack Query
  [https://tanstack.com/query](https://tanstack.com/query)
- Shadcn UI
  [https://ui.shadcn.com/](https://ui.shadcn.com/)
- React Hook Form
  [https://react-hook-form.com/](https://react-hook-form.com/)
- Zod
  [https://zod.dev/](https://zod.dev/)
- Tailwind CSS
  [https://tailwindcss.com/](https://tailwindcss.com/)
- Docker
  [https://www.docker.com/](https://www.docker.com/)
- GitHub Actions
  [https://github.com/features/actions](https://github.com/features/actions)

## Getting Started

### Prerequisites

- Halcyon API
  [https://github.com/chrispoulter/halcyon-dotnet](https://github.com/chrispoulter/halcyon-dotnet)

### Install dependencies

Install NPM packages:

```
npm install
```

### Configure environment variables

For local development, you'll need to create a `.env.local` file in the root of the project to define the environment variables. This file is ignored by Git, so the secrets will not be committed to the repository.

```
VITE_API_URL=http://localhost:5257
```

### Running the development server

Once the dependencies are installed, you can run the development server:

```
npm run dev
```

Open http://localhost:5173 in your browser to see the project running.

## Building for Production

To build the project for production:

```
npm run build
```

This command will create an optimized build in the `dist` folder.

## Linting & Formatting

To lint and format the code:

```
npm run lint
npm run format
```

## Contributing

Feel free to submit issues or pull requests to improve the template. Ensure that you follow the coding standards and test your changes before submission.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
