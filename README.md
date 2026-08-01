# backseat-driver

An LLM harness meant to imitate a "backseat driver" in pair programming.

Typically, there's a driver doing the mechanical work and a navigator doing the
mental work. A backseat driver[^1] in this case is a navigator who's acting on the
same level as the driver. This description fits the general idea of this harness.

It is not meant as a working agent, but rather as a second opinion on standby.
It can't actively interject like in a real pair programming situation, but it
will respond when asked.

## Roadmap

- [x] Core processing loop with external OpenAI compatible API
- [x] Tool call; reading files and listing directories
- [ ] ... more?

## Notes

I don't know shit about agentic coding. Never used Copilot, Claude, Codex, Cursor,
or whatever other C-tools there are, and don't really plan on changing that. Take
this repository as an attempt to "rediscover" this trend from first principles.
Maybe I'll use it, maybe I'll just abandon it because it's dumb. Who knows.

[^1]: Jones, D. L., & Fleming, S. D. (2013). What use is a backseat driver? A qualitative investigation of pair programming.
2013 IEEE Symposium on Visual Languages and Human Centric Computing, 103–110. https://doi.org/10.1109/vlhcc.2013.6645252
